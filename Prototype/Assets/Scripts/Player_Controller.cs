using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public static Player_Controller instance;
    //Class References
    private Controls controls;
    private Rigidbody rb;
    private Collider col;
    private Animator animator;

    public Player_Control_Settings settings;
    public Bounds Player_Bounds;
    public bool show_bounds;

    private Vector2 direction;
    private Vector2 detection;
    public bool isControlling = true;

    public enum State { Waiting = default, Grounded, Ceiling, Cling, Aerial }
    public State current_state;

    private void Awake()
    {
        instance = this;
        rb = TryGetComponent(out Rigidbody r) ? r : null;
        col = TryGetComponent(out Collider c) ? c : null;
        animator = TryGetComponent(out Animator a) ? a : null;      
    }
    private void OnEnable()
    {
        controls = controls == null ? new Controls() : controls;
        Subscribe_Actions();
        controls.Player.Enable();
    }

    #region MOVEMENT/AIR CONTROL
    private void Request_Movement(InputAction.CallbackContext context)
    {       
        Vector2 temp = context.ReadValue<Vector2>();
        float x =  Mathf.Abs(temp.x) > (settings.horizontal_deadzone / 100f) ? temp.x : 0.0f;
        float y = Mathf.Abs(temp.y) > (settings.vertical_deadzone / 100f) ? temp.y : 0.0f;      
        direction = new Vector2(x, y);
    }

    private void Movement()
    {      
        Flip((int)((direction.x % 1) + (direction.x / 1)));
        if (!isControlling) { return; }
        float speed;

        switch (current_state)
        {
            default: break;
            case State.Grounded:
                speed = direction.x * settings.move_speed;
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(speed, rb.velocity.y, 0), settings.move_smoothing);
                break;
            case State.Cling:
                if(direction.y < 0) { Wall_Slide(settings.wall_slide_speed); }
                if(direction.x != 0) {
                    speed = direction.x * settings.move_speed * settings.wall_release_strength / 100;
                    rb.velocity = new Vector3(speed, rb.velocity.y, 0);}
                break;
            case State.Aerial:
                speed = Mathf.Clamp(Mathf.Lerp(rb.velocity.x, rb.velocity.x + direction.x * settings.move_speed, settings.air_control), -settings.max_air_speed, settings.max_air_speed);
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(speed, rb.velocity.y, 0), settings.move_smoothing);
                break;
        }
    }
    #endregion

    #region JUMP
    private void Request_Jump(InputAction.CallbackContext context)
    {
        switch (current_state)
        {
            case State.Grounded:
                StartCoroutine(Jump()); animator.Play("Jump_01");
                break;
            case State.Cling:
                StartCoroutine(Wall_Jump()); animator.SetTrigger("Jump_03");
                break;
        }
    }   
    private IEnumerator Jump()
    {              
        float force;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * settings.jump_power, ForceMode.Force);
        float time = 0;
        while (time >= 0)
        {
            time += Time.deltaTime;
            force = -Mathf.Pow(time, 2) / (1/ settings.weight *10) + settings.floatiness;
            if (controls.Player.Jump.phase == InputActionPhase.Waiting) { force = -Mathf.Abs(force) * settings.downforce; }
            if (rb.velocity.y > -settings.max_fall_speed) { rb.AddForce(Vector3.up * force, ForceMode.Acceleration); };
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -settings.max_fall_speed, float.PositiveInfinity), rb.velocity.z);
            yield return new WaitForEndOfFrame();
            if (time > 0.2 && current_state != State.Aerial) { break; }
        }
    }
    #endregion

    #region WALL JUMP
    public IEnumerator Wall_Jump()
    {
        isControlling = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float angle = settings.wall_jump_angle * detection.x;
        Quaternion rotA = Quaternion.AngleAxis(angle, Vector3.forward);
        rb.AddForce((rotA * Vector3.up) * settings.wall_jump_power, ForceMode.Force);
        Debug.Log((rotA * Vector3.up));
        yield return new WaitForSeconds(0.15f);       
        isControlling = true;

        float force;
        float time = 0;
        while (time >= 0)
        {
            time += Time.deltaTime;
            force = -Mathf.Pow(time, 2) / (1 / settings.weight * 10) + settings.floatiness;
            if (controls.Player.Jump.phase == InputActionPhase.Waiting) { force = -Mathf.Abs(force) * settings.downforce; }
            if (rb.velocity.y > -settings.max_fall_speed) { rb.AddForce(Vector3.up * force, ForceMode.Acceleration); };
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -settings.max_fall_speed, float.PositiveInfinity), rb.velocity.z);
            yield return new WaitForEndOfFrame();
            if (current_state != State.Aerial) { break; }
        }
    }
    private float slide_time = 0;
    public void Wall_Grab()
    {
        rb.useGravity = false;
        Flip(Mathf.RoundToInt(detection.x));
        Vector2 dir = transform.position - (transform.position - (Vector3)detection);
        rb.AddForce(dir * settings.wall_grab_strength, ForceMode.Force);
        if (slide_time < settings.landing_slide_duration){slide_time += Time.deltaTime; Wall_Slide(settings.landing_slide_speed);}        
    }
    public void Wall_Slide(float speed)
    {        
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + Vector2.down * (speed / 10), settings.wall_slide_smoothing);
    }
    #endregion

    #region UTILITY
    private void Subscribe_Actions()
    {
        controls.Player.Move.performed += Request_Movement;
        controls.Player.Move.canceled += Request_Movement;
        controls.Player.Jump.performed += Request_Jump;
    }

    private State last_state = State.Waiting;
    private void Animation_Driver()
    {
        animator.SetFloat("Speed_X", Mathf.Abs((int)((direction.x % 1) + (direction.x / 1))));
        animator.SetFloat("Speed_Y", rb.velocity.y);
        animator.speed = rb.velocity.magnitude / settings.max_air_speed;
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

        if(last_state != current_state) { 
            last_state = current_state;
            Reset_Animation_Parameters();
        }
    }

    private void Reset_Animation_Parameters()
    {
        animator.speed = 1;
        //animator.SetBool("Ground", false);
        //animator.SetBool("Cling", false);
        //animator.ResetTrigger("Jump");
        //animator.ResetTrigger("Wall_Jump");
    }

    private void Flip(int direction)
    {
        switch (direction) {
            default:
                if (detection.x < 0) { Flip(1); }
                else if(detection.x > 0) { Flip(-1); }
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
        while(t > 0 && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
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
        if(last_state == State.Grounded && rb.velocity.y < 0)
        {
            StartCoroutine(Delay_State(State.Aerial, settings.coyote_jump_delay));
            return;
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
        float angle = Vector3.Angle(dir, -transform.up);
        detection = dir;
        float height = collision.GetContact(0).otherCollider.bounds.size.y;

        if (angle < settings.slope_angle) { current_state = State.Grounded; } // GROUND
        else if(angle > 180 - settings.ceiling_angle) { current_state = State.Ceiling; } // CEILING
        else if(angle > settings.slope_angle && angle < 180 - settings.ceiling_angle && height > settings.min_climb_height) { current_state = State.Cling; Wall_Grab(); } // WALL 
        else { current_state = State.Waiting; } // WAITING (DEFAULT)

        Debug.DrawLine(transform.position, transform.position + (p - transform.position).normalized, Color.green);
    }

    #endregion

    private void FixedUpdate()
    {
        Movement();
        Animation_Driver();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        #region SLOPE ANGLE BOUNDS
        Quaternion rot1 = Quaternion.AngleAxis(settings.slope_angle, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis((-settings.slope_angle), Vector3.forward);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot1 * Vector2.down));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot2 * Vector2.down));
        #endregion

        #region CEILING ANGLE BOUNDS
        Quaternion rot3 = Quaternion.AngleAxis(settings.ceiling_angle, Vector3.forward);
        Quaternion rot4 = Quaternion.AngleAxis((-settings.ceiling_angle), Vector3.forward);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot3 * Vector2.up));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot4 * Vector2.up));
        #endregion

        #region WALL JUMP ANGLE
        Gizmos.color = Color.blue;
        float angle = settings.wall_jump_angle * Mathf.RoundToInt(detection.x);
        Quaternion rot5 = Quaternion.AngleAxis(angle, Vector3.forward);
        Gizmos.DrawLine(transform.position, transform.position + (rot5 * Vector3.up));

        #endregion

        #region PLAYER BOUNDS
        if (show_bounds)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.85f, 0.4f);
            Gizmos.DrawCube(transform.position + Player_Bounds.center, Player_Bounds.size);
            Gizmos.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            Gizmos.DrawWireCube(transform.position + Player_Bounds.center, Player_Bounds.size);
        }
        #endregion
    }
}