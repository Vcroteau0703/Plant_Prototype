using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour, ISavable
{
    #region DECLARATIONS
    // SINGLETON INSTANCE //
    public static Player_Controller instance;

    //CLASS REFERENCES //
    public Controls controls;
    private Rigidbody rb;
    private Collider col;
    private Animator animator;
    public Control_Management c_manager;
    public Player_Control_Settings settings;
    public Player_Control_Settings[] setting_presets;

    // DEBUG INFO //
    [Header("Debug")]
    public Vector2 direction;
    public Vector2 detection;
    private float coyote_time;
    private float slide_speed = 0;
    private float aerial_time;
    public float target_gravity = -9.81f;

    // PLAYER STATES //
    public enum State { Waiting = default, Grounded, Ceiling, Cling, Aerial, Gliding, Sliding }
    [Header("States")]
    public State current_state;
    public State prev_state;
    #endregion

    private void Awake()
    {
        instance = this;
        rb = TryGetComponent(out Rigidbody r) ? r : null;
        col = TryGetComponent(out Collider c) ? c : null;
        animator = TryGetComponent(out Animator a) ? a : null;
        Control_Management manager_data = SaveSystem.Load<Control_Management>("/Player/Control_Manager.data");
        c_manager = manager_data != null ? manager_data : c_manager;
        //c_manager.Enable_All();
    }

    public void Save()
    {
        SaveSystem.Save(c_manager, "/Player/Control_Manager.data");
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
        target_gravity = Physics.gravity.y;
        Animation_Driver();
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
        if (!c_manager.Jumping.Enabled) { return; }
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
        if (context.started && rb.velocity.y < 0.1f && c_manager.Gliding.Enabled)
        {         
            current_state = State.Gliding;
        }
        else if (detection == Vector2.zero && current_state == State.Gliding && context.canceled)
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
                if (c_manager.Moving.Enabled) { Move(); return; }
                //Swap_States(ref current_state, ref prev_state);
                break;
            case State.Cling:
                coyote_time += Time.deltaTime;
                if (c_manager.Cling.Enabled) { Cling(); return; }
                Swap_States(ref current_state, ref prev_state);
                break;
            case State.Aerial:
                rb.useGravity = true;
                coyote_time += Time.deltaTime;
                if (c_manager.Aerials.Enabled) { Aerial(); return; }
                //Swap_States(ref current_state, ref prev_state);
                break;
            case State.Ceiling:
                if (c_manager.Ceiling.Enabled) { Ceiling(); return; }
                //Swap_States(ref current_state, ref prev_state);
                break;
            case State.Gliding:
                if (c_manager.Gliding.Enabled) { Glide(); Aerial(); return; }
                Swap_States(ref current_state, ref prev_state);
                break;
            case State.Sliding:
                if (c_manager.Sliding.Enabled && c_manager.Cling.Enabled) { Slide(); Cling(); return; }
                Swap_States(ref current_state, ref prev_state);
                break;
        }
    }
    public void Move()
    {
        if (!c_manager.Moving.Enabled) { return; }
        col.material.dynamicFriction = 0.1f;
        float ground_clamp = -0.75f;

        Vector2 dir = Quaternion.AngleAxis(90, Vector3.forward) * detection;

        float angle = Get_Detection_Angle();
        Vector2 temp = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        Debug.DrawLine(transform.position, (Vector2)transform.position + new Vector2(temp.x * direction.x, temp.y));


        float speed = direction.x * settings.Move_Speed;
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.Acceleration) * direction.x;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -Mathf.Abs(force), Mathf.Abs(force)), rb.velocity.y);

        rb.AddForce(new Vector2(temp.x * force, temp.y) + ground_clamp * Vector2.up);
        float friction_force = Mathf.Abs(direction.x) < settings.horizontal_deadzone ? Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(settings.Friction)) : 0;
        friction_force *= Mathf.Sign(rb.velocity.x);
        rb.AddForce(temp * -friction_force, ForceMode.Impulse);

        if(detection == Vector2.zero) { prev_state = State.Grounded; current_state = State.Aerial; }
    }   
    public void Cling()
    {
        float angle = Vector3.Angle(detection, -Vector3.up);
        if (angle < settings.Slope_Angle && settings.Wall_Jump.phase == Jump.State.Waiting && direction.x == 0)
        {
            current_state = State.Grounded; return;
        }

        //CLING
        rb.useGravity = false;
        Flip((int)((detection.x % 1) + (detection.x / 1)));
        Vector2 dir = transform.position - (transform.position - (Vector3)detection);
        rb.AddForce(dir * settings.Cling_Power, ForceMode.Force);

        //SLIDE CHECK
        if (direction.y < 0 && current_state != State.Sliding && detection != Vector2.zero && c_manager.Sliding.Enabled) { 
            current_state = State.Sliding; return;
        }

        //WALL EJECT
        rb.AddForce(direction.x * settings.Eject_Power * Vector3.right, ForceMode.Impulse);

        //SLIDE STATE LOCK
        if(current_state == State.Sliding) { return; }

        // AUTO SLIDE + FRICTION
        transform.position += Vector3.down * slide_speed * Time.deltaTime;
        if(slide_speed > 0){slide_speed -= Time.deltaTime * settings.Slide_Friction;}
        else { slide_speed = 0; }

        //STATE CHANGE
        if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay) { current_state = State.Cling; } // CLING 
        else { prev_state = State.Cling; current_state = State.Aerial; }

    }
    public void Aerial()
    {
        aerial_time += Time.deltaTime;
        slide_speed = rb.velocity.y < -settings.Slide_Threshhold && Mathf.Abs(rb.velocity.y) > slide_speed ? Mathf.Clamp(Mathf.Abs(rb.velocity.y), 0, settings.Max_Slide_Speed) : slide_speed;
        float speed = direction.x * settings.Air_Speed;
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.Air_Accel) * direction.x;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -settings.Air_Speed, settings.Air_Speed), rb.velocity.y);
        rb.AddForce(force * settings.Air_Control * Vector3.right, ForceMode.Acceleration);

        if (rb.velocity.y < -10f)
        {
            Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
        }
        else if (rb.velocity.y < 1 || settings.Jump.phase == Jump.State.Canceled || settings.Wall_Jump.phase == Jump.State.Canceled)
        {          
            Physics.gravity = new Vector3(Physics.gravity.x, settings.Fall_Speed, Physics.gravity.z);
        }
        else
        {
            Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
        }

        //STATE CHANGE//
        if(detection == Vector2.zero) { return; }
        float angle = Vector3.Angle(detection, -Vector3.up);
        if (angle < settings.Slope_Angle) { prev_state = State.Aerial; current_state = State.Grounded; aerial_time = 0; } // GROUND
        else if (angle > 180 - settings.Ceiling_Angle) { prev_state = State.Aerial; current_state = State.Ceiling; rb.useGravity = true; } // CEILING
        else if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay && c_manager.Cling.Enabled && aerial_time > 0.5f) 
        { 
            prev_state = State.Aerial;
            current_state = State.Cling;
            aerial_time = 0;
        } // CLING 
    }
    public void Ceiling()
    {
        rb.AddForce(-transform.up * settings.Bonk_Power, ForceMode.Impulse);
        current_state = State.Aerial;
        prev_state = State.Ceiling;
    }
    public void Glide()
    {
        rb.AddForce(transform.up * -Physics.gravity.y/(10/settings.Glide_Power), ForceMode.Force);
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -1 / settings.Glide_Power, float.PositiveInfinity), rb.velocity.z);

        if(detection == Vector2.zero) {return;}

        float angle = Vector3.Angle(detection, -Vector3.up);
        if (angle < settings.Slope_Angle){ prev_state = State.Gliding; current_state = State.Grounded; } // GROUND
        else if (angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay && c_manager.Cling.Enabled) { prev_state = State.Gliding; current_state = State.Cling; } // CLING 
    }
    public void Slide()
    {
        float angle = Vector3.Angle(detection, -Vector3.up);
        if (direction.y < 0 && detection != Vector2.zero)
        {
            float a = slide_speed + Time.deltaTime * settings.Slide_Accel;
            slide_speed = Mathf.Clamp(a, 0, settings.Max_Slide_Speed);
            transform.position += Vector3.down * slide_speed * Time.deltaTime;
            return;
        }
        else if(detection == Vector2.zero)
        {
            current_state = State.Aerial;
            prev_state = State.Sliding;
        }
        else if(angle > settings.Slope_Angle && angle < 180 - settings.Ceiling_Angle && coyote_time > settings.Coyote_Delay)
        {
            current_state = State.Cling; // CLING
            prev_state = State.Sliding;
        }
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
            if (force < 0.1f)
            {
                jump.phase = jump.phase != Jump.State.Waiting ? Jump.State.Canceled : Jump.State.Waiting;
                if (controls.Player.Glide.phase == InputActionPhase.Performed && c_manager.Gliding.Enabled) {
                    prev_state = State.Aerial;
                    current_state = State.Gliding;
                }
                break;
            }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        jump.phase = jump.phase != Jump.State.Waiting ? Jump.State.Canceled : Jump.State.Waiting;
    }
    public IEnumerator Jump_02(Jump jump, float time)
    {
        StartCoroutine(Jump_02_Buffer());
        jump.phase = Jump.State.Started;
        rb.velocity = new Vector2(rb.velocity.x, 0);

        float angle = detection.x > 0 ? Get_Detection_Angle() + 90 + settings.Wall_Jump.angle : (Get_Detection_Angle() + 90 + +settings.Wall_Jump.angle) * -1;
        Vector2 dir = Quaternion.AngleAxis(angle, Vector3.forward) * -Vector2.up;
        Debug.Log(angle);

        rb.AddForce(jump.power * dir, ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.power * 0.90f / (1 / jump.floatiness));
            if (force < 0.1f) {
                jump.phase = jump.phase != Jump.State.Waiting ? Jump.State.Canceled : Jump.State.Waiting;
                if (controls.Player.Glide.phase == InputActionPhase.Performed && c_manager.Gliding.Enabled) {
                    prev_state = State.Aerial; 
                    current_state = State.Gliding; 
                }
                break; }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        jump.phase = jump.phase != Jump.State.Waiting ? Jump.State.Canceled : Jump.State.Waiting;
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
        animator.SetInteger("DIR_X", (int)direction.x);
        animator.SetInteger("DIR_Y", (int)direction.y);
        animator.SetFloat("VELOCITY_Y", Mathf.Round(rb.velocity.y));
        animator.SetFloat("VELOCITY_X", Mathf.Round(rb.velocity.x));
        animator.SetFloat("WALK_SPEED", rb.velocity.x * direction.x);

        animator.SetBool("GROUND", current_state == State.Grounded);
        animator.SetBool("AERIAL", current_state == State.Aerial);
        animator.SetBool("CLING", current_state == State.Cling);
        animator.SetBool("GLIDE", current_state == State.Gliding);
        animator.SetBool("CEILING", current_state == State.Ceiling);
        animator.SetBool("SLIDE", current_state == State.Sliding);
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
    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    private float Get_Detection_Angle()
    {
        float angle;
        switch (current_state) 
        {
            case State.Grounded:
                angle = Mathf.Clamp(Vector3.Angle(detection, -Vector3.up), 0, settings.Slope_Angle);
                break;
            default:
                angle = Vector3.Angle(detection, -Vector3.up);
                break;
        }

        return angle;
    }
    private void Swap_States(ref State a, ref State b)
    {
        State temp = a;       
        a = b;
        b = temp;       
    }

    private void OnCollisionExit(Collision collision)
    {
        if(Mathf.Abs(direction.x) < 0.2f && (current_state == State.Cling || current_state == State.Sliding) ) { Flip(); }
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
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ENTER");
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
        Gizmos.color = Color.red;

        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position + detection);


        Gizmos.color = Color.yellow;

        #region SLOPE ANGLE BOUNDS
        Quaternion rot1 = Quaternion.AngleAxis(settings.Slope_Angle, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis((-settings.Slope_Angle), Vector3.forward);
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot1 * Vector2.down));
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot2 * Vector2.down));
        #endregion

        #region CEILING ANGLE BOUNDS
        Quaternion rot3 = Quaternion.AngleAxis(settings.Ceiling_Angle, Vector3.forward);
        Quaternion rot4 = Quaternion.AngleAxis((-settings.Ceiling_Angle), Vector3.forward);
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot3 * Vector2.up));
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot4 * Vector2.up));
        #endregion

        float angle = detection.x > 0 ? Get_Detection_Angle() + 90 + settings.Wall_Jump.angle : (Get_Detection_Angle() + 90 + +settings.Wall_Jump.angle) * -1;
        Vector2 dir = Quaternion.AngleAxis(angle, Vector3.forward) * -Vector2.up;

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)dir);
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

    public enum State { Waiting = default, Canceled, Started, Performing }
    public State phase;
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