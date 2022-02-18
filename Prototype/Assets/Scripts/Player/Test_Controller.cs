using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Controller : MonoBehaviour
{
    public static Test_Controller instance;
    //Class References
    private Controls controls;
    private Rigidbody rb;
    private Collider col;
    private Animator animator;

    public Player_Control_Settings settings;
    public Bounds Player_Bounds;
    public bool show_bounds;

    [Header("Debug")]
    public Player_Control_Settings[] setting_presets;
    private Vector2 direction;
    public Vector2 momentum;
    private Vector2 detection;
    public bool isControlling = true;
    public bool groundLock = true;
    private float coyote_time;
    public enum State { Waiting = default, Grounded, Ceiling, Cling, Aerial }
    public State current_state;
    private State last_state = default;

    private void Awake()
    {
        instance = this;
        rb = TryGetComponent(out Rigidbody r) ? r : null;
        col = TryGetComponent(out Collider c) ? c : null;
        animator = TryGetComponent(out Animator a) ? a : null;      
    }
    private void OnEnable()
    {
        settings = setting_presets.Length > 0 ? setting_presets[0] : settings;
        controls = controls == null ? new Controls() : controls;
        Subscribe_Actions();
        controls.Player.Enable();
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
        groundLock = false;
        switch (current_state)
        {
            case State.Grounded:
                if(settings.Jump.phase == Jump.State.Waiting) {
                    settings.Jump.phase = Jump.State.Started;
                    StartCoroutine(Jump_01(settings.Jump, 0));                    
                }           
                break;
            case State.Cling:
                if (settings.Wall_Jump.phase == Jump.State.Waiting) {
                    settings.Wall_Jump.phase = Jump.State.Started;
                    StartCoroutine(Jump_02(settings.Wall_Jump, 0));                    
                }
                break;
            case State.Aerial:
                if(coyote_time < settings.coyote_jump_delay && settings.Jump.phase == Jump.State.Waiting){
                    settings.Jump.phase = Jump.State.Started;
                    StartCoroutine(Jump_01(settings.Jump, 0));
                }
                break;
        }
    }
    private void Request_Cancel_Jump(InputAction.CallbackContext context)
    {
        if (settings.Jump.phase == Jump.State.Started){
            settings.Jump.phase = Jump.State.Canceled;
        }
        if (settings.Wall_Jump.phase == Jump.State.Started){
            settings.Wall_Jump.phase = Jump.State.Canceled;
        }
    }


    #endregion

    #region MOVEMENT

    public void State_Control()
    {
        switch(current_state)
        {
            case State.Grounded:
                //Ground_Lock();
                //settings.Jump.phase = Jump.State.Waiting;
                coyote_time = 0;
                if (direction.x != 0) { Move(); }
                break;
            case State.Cling:
                Cling();
                break;
            case State.Aerial:
                coyote_time += Time.deltaTime;
                Aerial();
                break;
        }
    }

    public void Move()
    {
        float speed = direction.x * settings.move_speed;
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.acceleration) * direction.x;        
        rb.AddForce(force * Vector3.right);
        float friction_force = Mathf.Abs(direction.x) < settings.horizontal_deadzone ? Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(settings.friction)) : 0;
        friction_force *= Mathf.Sign(rb.velocity.x);
        rb.AddForce(Vector3.right * -friction_force, ForceMode.Impulse);
    }
    private float slide_time = 0;
    public void Cling()
    {
        Flip((int)((detection.x % 1) + (detection.x / 1)));
        rb.useGravity = false;
        Vector2 dir = transform.position - (transform.position - (Vector3)detection);
        rb.AddForce(dir * settings.cling_strength, ForceMode.Force);
        if (slide_time < settings.landing_slide_duration) {
            transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + Vector2.down * (settings.landing_slide_speed / 10), settings.wall_slide_smoothing);
            slide_time += Time.deltaTime;
        }

        if (current_state == State.Cling)
        {
            rb.AddForce(direction.x * settings.cling_release_power * Vector3.right, ForceMode.Impulse);
        }
    }

    public void Aerial()
    {
        float speed = direction.x * settings.Air_Speed;
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.acceleration) * direction.x;
        rb.AddForce(force * settings.Air_Control * Vector3.right, ForceMode.Acceleration);

        if (rb.velocity.y < 1 || settings.Jump.phase == Jump.State.Canceled || settings.Wall_Jump.phase == Jump.State.Canceled)
        {
            Physics.gravity = new Vector3(Physics.gravity.x, settings.Fall_Speed, Physics.gravity.z);
        }
        else { Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z); }
    }

    public IEnumerator Jump_01(Jump jump, float time)
    {
        jump.phase = Jump.State.Started;
        rb.AddForce(jump.initial_power * Vector3.up, ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.initial_power*0.90f/ (1/jump.floatiness));
            if (force < 0.1f) { jump.phase = Jump.State.Canceled; break; }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);    
            yield return new WaitForFixedUpdate();
        }
        jump.phase = Jump.State.Canceled;
    }

    public IEnumerator Jump_02(Jump jump, float time)
    {
        jump.phase = Jump.State.Started;
        Debug.Log("Wall Jump");
        float angle = jump.angle * detection.x;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

        rb.AddForce(jump.initial_power * (rot * Vector3.up), ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {
            Debug.Log("LOOP");
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.initial_power * 0.90f / (1 / jump.floatiness));
            if (force < 0.1f) { jump.phase = Jump.State.Canceled; break; }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("Jump Stop");
        jump.phase = Jump.State.Canceled;
    }


    #endregion

    private void Ground_Lock()
    {       
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.position - (transform.position - (Vector3)detection), out hit, 1f))
        {
            if(hit.distance < 0.1f) { return; }
            transform.position += (hit.distance - col.bounds.extents.y) * Vector3.down;
        }
        Debug.DrawRay(transform.position, transform.position - (transform.position - (Vector3)detection), Color.red, 1f);
    }

    #region UTILITY
    private void Subscribe_Actions()
    {
        controls.Player.Move.performed += Request_Movement;
        controls.Player.Move.canceled += Request_Movement;
        controls.Player.Jump.performed += Request_Jump;
        controls.Player.Jump.canceled += Request_Cancel_Jump;
        controls.Player.Switch_Controls.performed += Switch_Settings;
    }
    private void Animation_Driver()
    {
        switch (current_state)
        {
            case State.Grounded:
                if(direction.x != 0) { animator.Play("Walk", 0); }
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
        switch (direction) {
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
        if (last_state == State.Cling)
        {
            Flip((int)((-detection.x % 1) + (-detection.x / 1)));
        }
        current_state = State.Aerial;
        rb.useGravity = true;
        slide_time = 0;
        detection = Vector3.zero;
    }
    private void OnCollisionStay(Collision collision)
    {
        Vector3[] points = new Vector3[collision.contactCount];

        for(int i = 0; i < collision.contactCount; i++)
        {
            points[i] = collision.GetContact(i).point;
        }

        float x = 0, y = 0;
        foreach(Vector3 point in points)
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

        if (angle < settings.slope_angle) { current_state = State.Grounded; } // GROUND
        else if(angle > 180 - settings.ceiling_angle) { current_state = State.Ceiling; rb.useGravity = true; } // CEILING
        else if(angle > settings.slope_angle && angle < 180 - settings.ceiling_angle && height > settings.min_climb_height) { current_state = State.Cling; } // WALL 
        else { current_state = State.Waiting; } // WAITING (DEFAULT)
        Debug.DrawLine(transform.position, transform.position + (average - transform.position).normalized, Color.green);
    }
    private void OnCollisionEnter(Collision collision)
    {
        slide_time = 0;
        settings.Jump.phase = Jump.State.Waiting;
        settings.Wall_Jump.phase = Jump.State.Waiting;
    }

    #endregion

    #region DEBUG

    private int index = 0;
    private void Switch_Settings(InputAction.CallbackContext context)
    {
        index = setting_presets.Length-1 > index ? index+=1 : 0;
        settings = setting_presets[index];
        Notification_System.Send_SystemNotify("Player control settings changed to " + settings.name, Color.blue);
    }

    #endregion

    private void FixedUpdate()
    {
        State_Control();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        //settings.Jump.phase = Jump.State.Waiting;
        //settings.Wall_Jump.phase = Jump.State.Waiting;
    }
}
