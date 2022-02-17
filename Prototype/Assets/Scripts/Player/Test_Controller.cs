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
                    StartCoroutine(Jump_01(settings.Jump, 0));                 
                }           
                break;
            case State.Cling:
                if (settings.Wall_Jump.phase == Jump.State.Waiting) { 
                    Jump_02(settings.Wall_Jump, 0);
                    settings.Wall_Jump.phase = Jump.State.Started;
                }
                break;
        }
    }
    private void Request_Cancel_Jump(InputAction.CallbackContext context)
    {
        settings.Jump.phase = Jump.State.Waiting;
        settings.Wall_Jump.phase = Jump.State.Waiting;   
    }


    #endregion

    #region MOVEMENT

    public void State_Control()
    {
        switch(current_state)
        {
            case State.Grounded:
                //Ground_Lock();
                if (direction.x != 0) { Move(); }
                break;
            case State.Cling:
                Cling();
                break;
            case State.Aerial:
                Aerial();
                break;
        }
    }

    public void Move()
    {
        float speed = -direction.x * settings.move_speed;
        float diff = Mathf.Abs(rb.velocity.x) - Mathf.Abs(speed);
        rb.AddForce(speed * diff * settings.acceleration * Vector3.right);
    }

    public void Cling()
    {
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.position - (transform.position - (Vector3)detection), out hit, 1f))
        //{
        //    if (hit.distance < 0.1f) { return; }
        //    transform.position += (hit.distance - col.bounds.extents.x) * Vector3.right;
        //}
    }

    public void Aerial()
    {
        float speed = -direction.x * settings.Air_Speed;
        float diff = Mathf.Abs(rb.velocity.x) - Mathf.Abs(speed);
        rb.AddForce(speed * diff * settings.Air_Control * Vector3.right, ForceMode.Acceleration);

        Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
        if (rb.velocity.y < 0)
        {
            Physics.gravity = new Vector3(Physics.gravity.x, settings.Fall_Speed, Physics.gravity.z);
        }
    }

    public IEnumerator Jump_01(Jump jump, float time)
    {
        jump.phase = Jump.State.Started;
        rb.AddForce(jump.initial_power * Vector3.up, ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.initial_power*0.90f/ (1/jump.floatiness));
            if (force < 0.1f) { jump.phase = Jump.State.Waiting; break; }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);    
            yield return new WaitForFixedUpdate();
        }
        jump.phase = Jump.State.Waiting;
    }

    public void Jump_02(Jump jump, float time)
    {
        time += Time.fixedDeltaTime;
        float force = (jump.max_height / time) * Mathf.Sqrt(jump.initial_power / 10f) - jump.floatiness;
        Vector2 dir = (force * Vector3.up) + ((rb.velocity.x + time) * Vector3.right);
        rb.AddForce(dir, ForceMode.Acceleration);
        jump.phase = Jump.State.Performing;
        if (current_state != State.Aerial || controls.Player.Jump.phase == InputActionPhase.Waiting || force <= 0) { jump.phase = Jump.State.Waiting; return; }
        Jump_02(jump, time);
    }


    #endregion


//    private void Movement()
//    {
//        if (groundLock) { Ground_Lock(); }
//        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -settings.max_fall_speed, rb.velocity.y));
//        Flip((int)((direction.x % 1) + (direction.x / 1)));
//        if (!isControlling) { return; }
//        float speed;
//        switch (current_state)
//        {
//            default: break;
//            case State.Grounded:
//                Jump.Phase = Jump.State.Waiting;
//                Wall_Jump.Phase = Jump.State.Waiting;
//                speed = direction.x * settings.move_speed;
//                float force = Mathf.Abs(direction.x) > 0.01f ? Mathf.Pow(Mathf.Abs(speed - rb.velocity.x), settings.acceleration) : 0;
//                rb.AddForce((force * direction.x) * Vector3.right, ForceMode.Acceleration);
//                float friction_force = direction.x < settings.horizontal_deadzone ? Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(settings.friction)) : 0;
//                friction_force *= Mathf.Sign(rb.velocity.x);
//                rb.AddForce(Vector3.right * -friction_force, ForceMode.Impulse);
//                break;
//            case State.Cling:
//                Jump.Phase = Jump.State.Waiting;
//                Wall_Jump.Phase = Jump.State.Waiting;
//                groundLock = false;
//                if (direction.y < 0) { Wall_Slide(settings.wall_slide_speed); }
//                if(direction.x != 0) {
//                    speed = direction.x * settings.move_speed * settings.wall_release_strength / 100;
//                    rb.velocity = new Vector3(speed, rb.velocity.y, 0);}
//                break;
//            case State.Aerial:
//                if(rb.velocity.y < settings.max_fall_speed)
//{
//                    rb.AddForce(Vector3.down * Mathf.Abs(settings.max_fall_speed - rb.velocity.y), ForceMode.Acceleration);
//                }
//                groundLock = true;
//                speed = direction.x * settings.air_control;
//                float accel = Mathf.Abs(direction.x) > 0.01f ? speed : (rb.velocity.x - Time.deltaTime) * settings.horizontal_momentum;
//                Debug.Log(accel);
//                rb.AddForce(accel * transform.right, ForceMode.Acceleration);
//                rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -settings.max_air_speed, settings.max_air_speed), rb.velocity.y, rb.velocity.z);
//                break;
//        }      
//    }

    private void Ground_Lock()
    {       
        //if(direction.x == 0) { rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z); }
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.position - (transform.position - (Vector3)detection), out hit, 1f))
        {
            if(hit.distance < 0.1f) { return; }
            transform.position += (hit.distance - col.bounds.extents.y) * Vector3.down;
        }
        Debug.DrawRay(transform.position, transform.position - (transform.position - (Vector3)detection), Color.red, 1f);
    }

    #region JUMP

    #region VERTICAL JUMP
    //private IEnumerator Vertical_Jump_Action()
    //{
    //    Jump.Phase = Jump.State.Started;
    //    StartCoroutine(Jump_Buffer(settings.jump_buffer));
    //    float time = 0;
    //    while (time >= 0)
    //    {
    //        time += Time.deltaTime;
    //        float force = (settings.jump_height / time) * Mathf.Sqrt(settings.jump_power/10f) - settings.weight;
    //        Vector2 dir = force * Vector3.up;
    //        rb.AddForce(dir, ForceMode.Acceleration);
    //        if (controls.Player.Jump.phase == InputActionPhase.Waiting || force <= 0) { Jump.Phase = Jump.State.Canceled; break; }

    //        yield return new WaitForFixedUpdate();
    //        Jump.Phase = Jump.State.Performing;
    //        if (current_state != State.Aerial && Jump.Enabled){Jump.Phase = Jump.State.Waiting; break; }
    //    }
    //}
    //public IEnumerator Jump_Buffer(float amount)
    //{
    //    Jump.Enabled = false;
    //    yield return new WaitForSeconds(amount);
    //    Jump.Enabled = true;
    //}   
    #endregion

    #region WALL JUMP
    //public IEnumerator Wall_Jump_Action()
    //{
    //    Wall_Jump.Phase = Jump.State.Started;
    //    StartCoroutine(Wall_Jump_Buffer(settings.wall_jump_buffer));
    //    float time = 0;
    //    while (time >= 0)
    //    {
    //        time += Time.deltaTime;
    //        float angle = settings.wall_jump_angle * detection.x;
    //        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

    //        float force = (settings.wall_jump_height / time) * Mathf.Sqrt(settings.wall_jump_power/10f) - settings.weight;
    //        Vector2 dir = (force * (rot * transform.up) * settings.wall_jump_power);
    //        Debug.DrawLine(transform.position, (Vector2)transform.position + dir, Color.blue);
    //        if (controls.Player.Jump.phase == InputActionPhase.Waiting || force <= 0) { Wall_Jump.Phase = Jump.State.Canceled; break; }


    //        //rb.AddForce((rot * transform.up) * settings.wall_jump_power, ForceMode.Force);

    //        rb.AddForce(dir, ForceMode.Acceleration);
    //        yield return new WaitForFixedUpdate();
    //        Wall_Jump.Phase = Jump.State.Performing;
    //        if (current_state != State.Aerial && Wall_Jump.Enabled) { Wall_Jump.Phase = Jump.State.Waiting; Debug.Log("STOP"); break; }
    //    }  
    //}
    public IEnumerator Wall_Jump_Buffer(float amount)
    {
        isControlling = false; settings.Wall_Jump.Enabled = false;
        yield return new WaitForSeconds(amount);
        isControlling = true; settings.Wall_Jump.Enabled = true;
    }

    private float slide_time = 0;
    //public void Wall_Grab()
    //{
    //    Flip((int)((detection.x % 1) + (detection.x / 1)));
    //    rb.useGravity = false;
    //    Vector2 dir = transform.position - (transform.position - (Vector3)detection);
    //    rb.AddForce(dir * settings.wall_grab_strength, ForceMode.Force);
    //    if (slide_time < settings.landing_slide_duration){slide_time += Time.deltaTime; Wall_Slide(settings.landing_slide_speed);}        
    //}
    public void Wall_Slide(float speed)
    {
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + Vector2.down * (speed / 10), settings.wall_slide_smoothing);
    }
    #endregion

    #endregion

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
    private IEnumerator Delay_State(State state, float t)
    {
        while(t > 0 && settings.Jump.Enabled)
        {
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        current_state = state;
        rb.useGravity = true;
        slide_time = 0;
        detection = Vector3.zero;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(current_state == State.Grounded)
        {
            StartCoroutine(Delay_State(State.Aerial, settings.coyote_jump_delay));
            return;
        }

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
        Vector3 p = collision.GetContact(0).point;
        Vector3 dir = p - transform.position;     
        float angle = Vector3.Angle(dir, -Vector3.up);
        detection = dir;
        float height = collision.GetContact(0).otherCollider.bounds.size.y;

        if (angle < settings.slope_angle) { current_state = State.Grounded; } // GROUND
        else if(angle > 180 - settings.ceiling_angle) { current_state = State.Ceiling; rb.useGravity = true; } // CEILING
        else if(angle > settings.slope_angle && angle < 180 - settings.ceiling_angle && height > settings.min_climb_height) { current_state = State.Cling; } // WALL 
        else { current_state = State.Waiting; } // WAITING (DEFAULT)
        Debug.DrawLine(transform.position, transform.position + (p - transform.position).normalized, Color.green);
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
