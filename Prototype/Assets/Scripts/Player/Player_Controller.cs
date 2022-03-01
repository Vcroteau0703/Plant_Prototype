using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region DECLARATIONS
    // SINGLETON INSTANCE //
    public static Player_Controller instance;

    //CLASS REFERENCES //
    private Controls controls;
    private Rigidbody rb;
    private Collider col;
    private Animator animator;
    public Control_Management c_manager;
    public Player_Control_Settings settings;
    public Player_Control_Settings[] setting_presets;

    // DEBUG INFO //
    [Header("Debug")]
    private Vector2 direction;
    public Vector2 detection;
    private float coyote_time;
    private float slide_speed = 0;
    private float aerial_time;
    public float target_gravity = -9.81f;

    // PLAYER STATES //
    public enum State { Waiting = default, Grounded, Ceiling, Cling, Aerial, Gliding, Sliding }
    [Header("States")]
    public State current_state;
    #endregion

    private void Awake()
    {
        instance = this;
        rb = TryGetComponent(out Rigidbody r) ? r : null;
        col = TryGetComponent(out Collider c) ? c : null;
        animator = TryGetComponent(out Animator a) ? a : null;
        //c_manager.Enable_All();
    }
    // Putting these here to allow me to call them within cutscenes and dialog
    public void EnableCling()
    {
        c_manager.Cling.Enable();
    }
    public void EnableGlide()
    {
        c_manager.Gliding.Enable();
    }
    private void OnEnable()
    {
        settings = setting_presets.Length > 0 ? setting_presets[0] : settings;
        controls = controls == null ? new Controls() : controls;
        Subscribe_Actions();
        controls.Player.Enable();
    }
    private void Start()
    {
        current_state = State.Aerial;
    }
    private void FixedUpdate()
    {
        State_Control();
        Animation_Driver();
    }
    private void Update()
    {
        //Animation_Driver();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    #region INPUT REQUESTS
    private void Request_Movement(InputAction.CallbackContext context)
    {
        Vector2 temp = context.ReadValue<Vector2>();
        float x = Mathf.Abs(temp.x) > (settings.horizontal_deadzone / 100f) ? temp.x : 0.0f;
        float y = Mathf.Abs(temp.y) > (settings.vertical_deadzone / 100f) ? temp.y : 0.0f;
        direction = new Vector2(x, y);
    }
    private void Request_Jump(InputAction.CallbackContext context)
    {
        switch (current_state)
        {
            case State.Grounded:
                if (settings.Jump.phase == Jump.State.Waiting)
                {
                    settings.Jump.phase = Jump.State.Started;
                    StartCoroutine(Jump_01(settings.Jump, 0));
                }
                break;
            case State.Cling:
                if (settings.Wall_Jump.phase == Jump.State.Waiting && coyote_time > settings.Coyote_Delay)
                {
                    settings.Wall_Jump.phase = Jump.State.Started;
                    StartCoroutine(Jump_02(settings.Wall_Jump, 0));
                }
                break;
            case State.Sliding:
                if (settings.Wall_Jump.phase == Jump.State.Waiting && coyote_time > settings.Coyote_Delay)
                {
                    settings.Wall_Jump.phase = Jump.State.Started;
                    StartCoroutine(Jump_02(settings.Wall_Jump, 0));
                }
                break;
            case State.Aerial:
                if (coyote_time < settings.Coyote_Delay && settings.Jump.phase == Jump.State.Waiting) {
                    settings.Jump.phase = Jump.State.Started;
                    StartCoroutine(Jump_01(settings.Jump, 0));
                }
                break;
        }
    }
    private void Request_Cancel_Jump(InputAction.CallbackContext context)
    {
        if (settings.Jump.phase == Jump.State.Started)
        {
            settings.Jump.phase = Jump.State.Canceled;
        }
        if (settings.Wall_Jump.phase == Jump.State.Started)
        {
            settings.Wall_Jump.phase = Jump.State.Canceled;
        }

        switch (current_state)
        {
            default:
                return;

            case State.Gliding:
                current_state = State.Aerial;
                break;
        }
    }
    private void Request_Glide(InputAction.CallbackContext context)
    {
        if (context.started)
        {         
            current_state = State.Gliding;
        }
        else if (detection == Vector2.zero && current_state == State.Gliding)
        {
            current_state = State.Aerial;
        }
    }
    #endregion

    #region STATE CONTROL
    public void State_Control()
    {
        Flip((int)((direction.x % 1) + (direction.x / 1)));
        switch (current_state)
        {
            case State.Grounded:
                coyote_time = 0;
                if (c_manager.Moving.Enabled) { Move(); }
                else { col.material.dynamicFriction = 5f; }
                break;
            case State.Cling:
                coyote_time += Time.deltaTime;
                if (c_manager.Cling.Enabled) { Cling(); }
                if (c_manager.Aerials.Enabled) { rb.velocity = new Vector2(rb.velocity.x, 0); settings.Wall_Jump.phase = Jump.State.Waiting; }             
                break;
            case State.Aerial:
                rb.useGravity = true;
                coyote_time += Time.deltaTime;
                if (c_manager.Aerials.Enabled) { Aerial(); }
                break;
            case State.Ceiling:
                if (c_manager.Ceiling.Enabled) { Ceiling(); }
                break;
            case State.Gliding:
                if (c_manager.Gliding.Enabled) { Glide(); Aerial(); }
                break;
            case State.Sliding:
                if (c_manager.Sliding.Enabled && c_manager.Cling.Enabled) { Slide(); Cling(); }
                break;

        }
    }
    public void Move()
    {
        if (!c_manager.Moving.Enabled) { return; }
        col.material.dynamicFriction = 0.1f;
        float ground_clamp = -0.75f;

        Vector2 dir = Quaternion.AngleAxis(90, Vector3.forward) * detection;

        float speed = direction.x * settings.Move_Speed;
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.Acceleration) * direction.x;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -Mathf.Abs(force), Mathf.Abs(force)), rb.velocity.y);

        rb.AddForce(force * dir + ground_clamp * Vector2.up);
        float friction_force = Mathf.Abs(direction.x) < settings.horizontal_deadzone ? Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(settings.Friction)) : 0;
        friction_force *= Mathf.Sign(rb.velocity.x);
        rb.AddForce(dir * -friction_force, ForceMode.Impulse);

        if(detection == Vector2.zero) { current_state = State.Aerial; }
    }   
    public void Cling()
    {
        //CLING
        rb.useGravity = false;
        Flip((int)((detection.x % 1) + (detection.x / 1)));
        Vector2 dir = transform.position - (transform.position - (Vector3)detection);
        rb.AddForce(dir * settings.Cling_Power, ForceMode.Force);

        //SLIDE CHECK
        if (direction.y < 0) { current_state = State.Sliding; return; }

        //WALL EJECT
        rb.AddForce(direction.x * settings.Eject_Power * Vector3.right, ForceMode.Impulse);

        // AUTO SLIDE + FRICTION
        transform.position += Vector3.down * slide_speed * Time.deltaTime;
        if(slide_speed > 0){slide_speed -= Time.deltaTime * settings.Slide_Friction;}
        else { slide_speed = 0; }

        //STATE CHANGE
        float angle = Vector3.Angle(detection, -Vector3.up);
        if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay) { current_state = State.Cling; } // CLING 
        else { current_state = State.Aerial; }

    }
    public void Aerial()
    {
        aerial_time += Time.deltaTime;
        slide_speed = rb.velocity.y < -settings.Slide_Threshhold && Mathf.Abs(rb.velocity.y) > slide_speed ? Mathf.Clamp(Mathf.Abs(rb.velocity.y), 0, settings.Max_Slide_Speed) : slide_speed;
        float speed = direction.x * settings.Air_Speed;
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.Air_Accel) * direction.x;
        rb.AddForce(force * settings.Air_Control * Vector3.right, ForceMode.Acceleration);

        if (rb.velocity.y < -10f)
        {
            Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
        }
        else if (rb.velocity.y < 1 || settings.Jump.phase == Jump.State.Canceled || settings.Wall_Jump.phase == Jump.State.Canceled)
        {          
            Physics.gravity = new Vector3(Physics.gravity.x, settings.Fall_Speed, Physics.gravity.z);
        }

        //STATE CHANGE//
        if(detection == Vector2.zero) { return; }
        aerial_time = 0;
        float angle = Vector3.Angle(detection, -Vector3.up);
        if (angle < settings.Slope_Angle) { current_state = State.Grounded; } // GROUND
        else if (angle > 180 - settings.Ceiling_Angle) { current_state = State.Ceiling; rb.useGravity = true; } // CEILING
        else if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay) { current_state = State.Cling; } // CLING 
    }
    public void Ceiling()
    {
        rb.AddForce(-transform.up * settings.Bonk_Power, ForceMode.Impulse);
        current_state = State.Aerial;
    }
    public void Glide()
    {
        Debug.Log("GLIDE");
        rb.AddForce(transform.up * -Physics.gravity.y/(10/settings.Glide_Power), ForceMode.Force);
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -1 / settings.Glide_Power, float.PositiveInfinity), rb.velocity.z);

        if(detection == Vector2.zero) {return;}

        float angle = Vector3.Angle(detection, -Vector3.up);
        if (angle < settings.Slope_Angle) { current_state = State.Grounded; } // GROUND
        else if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay) { current_state = State.Cling; } // CLING 
    }
    public void Slide()
    {
        if (direction.y < 0 && controls.Player.Jump.phase != InputActionPhase.Started)
        {
            float a = slide_speed + Time.deltaTime * settings.Slide_Accel;
            slide_speed = Mathf.Clamp(a, 0, settings.Max_Slide_Speed);
            transform.position += Vector3.down * slide_speed * Time.deltaTime;
            return;
        }

        
        //STATE CHANGE
        float angle = Vector3.Angle(detection, -Vector3.up);
        if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay) { current_state = State.Cling; } // CLING 
        else { current_state = State.Aerial; }
    }
    public IEnumerator Jump_01(Jump jump, float time)
    {
        jump.phase = Jump.State.Started;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jump.power * Vector3.up, ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.power * 0.90f / (1 / jump.floatiness));
            if (force < 0.1f) { 
                jump.phase = Jump.State.Canceled;
                if(controls.Player.Glide.phase == InputActionPhase.Performed) { current_state = State.Gliding; }
                break; }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        jump.phase = Jump.State.Canceled;
    }
    public IEnumerator Jump_02(Jump jump, float time)
    {
        StartCoroutine(Jump_02_Buffer());
        jump.phase = Jump.State.Started;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        float angle = jump.angle * detection.x;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

        rb.AddForce(jump.power * (rot * Vector3.up), ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.power * 0.90f / (1 / jump.floatiness));
            if (force < 0.1f) { 
                jump.phase = Jump.State.Canceled;
                if (controls.Player.Glide.phase == InputActionPhase.Performed) { current_state = State.Gliding; }
                break; }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        jump.phase = Jump.State.Canceled;
    }
    public IEnumerator Jump_02_Buffer()
    {
        c_manager.Aerials.Disable();
        yield return new WaitForSeconds(settings.Wall_Jump.buffer);
        c_manager.Aerials.Enable();
    }
    #endregion

    #region DEPRECATED FUNCTIONS
    private void Ground_Lock()
    {
        RaycastHit hit;
        Vector2 dir = detection != Vector2.zero ? detection : Vector2.down;
        Debug.DrawRay(transform.position, transform.position - (transform.position - (Vector3)dir * 1.5f), Color.red, 1.5f);
        if (Physics.Raycast(transform.position, transform.position - (transform.position - (Vector3)dir * 1.5f), out hit, 1.5f))
        {
            //if(((hit.distance - col.bounds.extents.y) * Vector3.down).y < 0.01f) { return; }          
            transform.position += (hit.distance - col.bounds.extents.y) * Vector3.down;
            Debug.Log((hit.distance - col.bounds.extents.y) * Vector3.down);
        }
    }
    #endregion

    #region UTILITY
    private void Subscribe_Actions()
    {
        controls.Player.Move.performed += Request_Movement;
        controls.Player.Move.canceled += Request_Movement;
        controls.Player.Jump.performed += Request_Jump;
        controls.Player.Jump.canceled += Request_Cancel_Jump;
        controls.Player.Glide.started += Request_Glide;
        controls.Player.Glide.canceled += Request_Glide;

        controls.Player.Switch_Controls.performed += Switch_Settings;
    }
    private void Animation_Driver()
    {
        switch (current_state)
        {
            case State.Grounded:
                if (direction.x != 0) { animator.Play("Walk", 0); }
                else { animator.speed = 1; animator.Play("Idle"); }
                break;
            case State.Aerial:
                if (rb.velocity.y < 0) { animator.Play("Fall", 0); }
                else { animator.Play("Aerial", 0); }
                break;
            case State.Cling:
                animator.speed = 1;
                animator.Play("Cling", 0);
                break;
            case State.Ceiling:
                break;
        }
    }
    private void Flip(int direction)
    {
        switch (direction)
        {
            default:
                break;
            case 1:
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;
            case -1:
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        rb.useGravity = true;
        detection = Vector2.zero;
    }
    private void OnCollisionStay(Collision collision)
    {
        Vector3[] points = new Vector3[collision.contactCount];

        for (int i = 0; i < collision.contactCount; i++)
        {
            points[i] = collision.GetContact(i).point;
        }

        float x = 0, y = 0;
        foreach (Vector3 point in points)
        {
            x += point.x;
            y += point.y;
        }

        Vector3 average = new Vector3(x / points.Length, y / points.Length, 0);

        //Vector3 p = collision.GetContact(0).point;
        Vector3 dir = average - transform.position;
        float angle = Vector3.Angle(dir, -Vector3.up);
        detection = dir;
        float height = collision.GetContact(0).otherCollider.bounds.size.y;


        //if (angle < settings.Slope_Angle) { current_state = State.Grounded; } // GROUND
        //else if (angle > 180 - settings.Ceiling_Angle) { current_state = State.Ceiling; rb.useGravity = true; } // CEILING
        //else if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && height > settings.Cling_Threshold && coyote_time > settings.Coyote_Delay) { current_state = State.Cling; } // WALL 
        //else { current_state = State.Aerial; } // WAITING (DEFAULT)
        Debug.DrawLine(transform.position, transform.position + (average - transform.position).normalized, Color.green);
    }
    private void OnCollisionEnter(Collision collision)
    {
        settings.Jump.phase = Jump.State.Waiting;
        settings.Wall_Jump.phase = Jump.State.Waiting;
    }

    #endregion

    #region DEBUG

    private int index = 0;
    private void Switch_Settings(InputAction.CallbackContext context)
    {
        index = setting_presets.Length - 1 > index ? index += 1 : 0;
        settings = setting_presets[index];
        Notification_System.Send_SystemNotify("Player control settings changed to " + settings.name, Color.blue);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        #region SLOPE ANGLE BOUNDS
        Quaternion rot1 = Quaternion.AngleAxis(settings.Slope_Angle, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis((-settings.Slope_Angle), Vector3.forward);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot1 * Vector2.down));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot2 * Vector2.down));
        #endregion

        #region CEILING ANGLE BOUNDS
        Quaternion rot3 = Quaternion.AngleAxis(settings.Ceiling_Angle, Vector3.forward);
        Quaternion rot4 = Quaternion.AngleAxis((-settings.Ceiling_Angle), Vector3.forward);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot3 * Vector2.up));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot4 * Vector2.up));
        #endregion
    }
    #endregion
}

#region CLASSES
[System.Serializable]
public class Jump
{
    public float power;
    public float max_height;
    public float floatiness;
    public float angle;
    public float buffer;

    public enum State { Canceled = default, Waiting, Started, Performing }
    public State phase = default;
    public Jump()
    {
        phase = default;
    }
}

[System.Serializable]
public class Control_Management
{
    public Control_State Moving, Gliding, Jumping, Aerials, Sliding, Cling, Ceiling;
    public void Disable_All() 
    {
        Moving.Disable();
        Gliding.Disable();
        Jumping.Disable();
        Aerials.Disable();
        Sliding.Disable();
        Cling.Disable();
        Ceiling.Disable();
    }
    public void Enable_All()
    {
        Moving.Enable();
        Gliding.Enable();
        Jumping.Enable();
        Aerials.Enable();
        Sliding.Enable();
        Cling.Enable();
        Ceiling.Enable();
    }
}

[System.Serializable]
public class Control_State
{
    public bool m_enabled;
    public bool Enabled { get { return m_enabled; } }

    public void Enable()
    {
        m_enabled = true;
    }
    public void Disable()
    {
        m_enabled = false;
    }

}
#endregion