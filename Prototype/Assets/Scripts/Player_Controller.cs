using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    //Class References
    private Controls controls;
    private Rigidbody rb;
    private Collider col;
    private Animator animator;

    public Bounds Player_Bounds;

    #region MOVEMENT VARIABLES
    [Header("Movement Settings")]
    public float move_speed = 1.0f;
    public float move_smoothing = 0.5f;
    public float downforce = 1f;
    public LayerMask walkable;
    [Header("Jump Settings")]
    public float jump_power = 1.0f;
    public float floatiness, weight;   
    public float coyote_jump_delay = 0.15f;
    [Header("Air Control Settings")]
    public float air_control = 1.0f;
    public float max_air_speed = 10.0f;
    public float max_fall_speed = 20.0f;
    [Header("Wall Jump Settings")]
    public float wall_jump_power = 10.0f;
    public float wall_grab_strength = 20.0f; 
    public float wall_jump_angle = 45.0f;
    public float wall_release_strength = 5.0f;
    [Header("Wall Slide Settings")]
    public float landing_slide_duration = 2.0f;
    public float landing_slide_speed = 5.0f;
    public float wall_slide_speed = 5.0f;
    public float wall_slide_smoothing = 0.1f;
    [Header("Detection Settings")]
    public float slope_angle = 30f;
    public float ceiling_angle = 30f;
    public float min_climb_height = 1f;
    [Header("Input Settings")]
    [Range(0f, 100f)] public float horizontal_deadzone = 20f;
    [Range(0f, 100f)] public float vertical_deadzone = 20f;
    
    private Vector2 direction;
    private Vector2 detection;
    public bool isControlling = true;

    public enum State { Waiting = default, Grounded, Ceiling, Cling, Aerial }
    public State current_state;


    #endregion

    private void Awake()
    {
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
        float x =  Mathf.Abs(temp.x) > (horizontal_deadzone / 100f) ? temp.x : 0.0f;
        float y = Mathf.Abs(temp.y) > (vertical_deadzone / 100f) ? temp.y : 0.0f;      
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
                speed = direction.x * move_speed;
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(speed, rb.velocity.y, 0), move_smoothing);
                break;
            case State.Cling:
                if(direction.y < 0) { Wall_Slide(wall_slide_speed); }
                if(direction.x != 0) {
                    speed = direction.x * move_speed * wall_release_strength / 100;
                    rb.velocity = new Vector3(speed, rb.velocity.y, 0);}
                break;
            case State.Aerial:
                speed = Mathf.Clamp(Mathf.Lerp(rb.velocity.x, rb.velocity.x + direction.x * move_speed, air_control), -max_air_speed, max_air_speed);
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(speed, rb.velocity.y, 0), move_smoothing);
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
        rb.AddForce(Vector3.up * jump_power, ForceMode.Force);
        float time = 0;
        while (time >= 0)
        {
            time += Time.deltaTime;
            force = -Mathf.Pow(time, 2) / (1/weight*10) + floatiness;
            if (controls.Player.Jump.phase == InputActionPhase.Waiting) { force = -Mathf.Abs(force) * downforce; }
            if (rb.velocity.y > -max_fall_speed) { rb.AddForce(Vector3.up * force, ForceMode.Acceleration); };
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -max_fall_speed, float.PositiveInfinity), rb.velocity.z);
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
        float angle = wall_jump_angle * Mathf.RoundToInt(detection.x);
        Quaternion rotA = Quaternion.AngleAxis(angle, Vector3.forward);
        rb.AddForce(transform.position + (rotA * Vector3.up) * wall_jump_power, ForceMode.Force);
        yield return new WaitForSeconds(0.15f);       
        isControlling = true;

        float force;
        float time = 0;
        while (time >= 0)
        {
            time += Time.deltaTime;
            force = -Mathf.Pow(time, 2) / (1 / weight * 10) + floatiness;
            if (controls.Player.Jump.phase == InputActionPhase.Waiting) { force = -Mathf.Abs(force) * downforce; }
            if (rb.velocity.y > -max_fall_speed) { rb.AddForce(Vector3.up * force, ForceMode.Acceleration); };
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -max_fall_speed, float.PositiveInfinity), rb.velocity.z);
            yield return new WaitForEndOfFrame();
            if (current_state != State.Aerial) { break; }
        }
    }
    private float slide_time = 0;
    public void Wall_Grab()
    {
        rb.useGravity = false;
        Flip(Mathf.RoundToInt(detection.x));
        rb.AddForce(detection * wall_grab_strength, ForceMode.Force);
        if (slide_time < landing_slide_duration){slide_time += Time.deltaTime; Wall_Slide(landing_slide_speed);}        
    }
    public void Wall_Slide(float speed)
    {        
        transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + Vector2.down * (speed / 10), wall_slide_smoothing);
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
        animator.speed = rb.velocity.magnitude / max_air_speed;
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


    private void OnCollisionExit(Collision collision)
    {
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
        Debug.Log("height: " + height);

        if (angle < slope_angle) { current_state = State.Grounded; } // GROUND
        else if(angle > 180 - ceiling_angle) { current_state = State.Ceiling; } // CEILING
        else if(angle > slope_angle && angle < 180 - ceiling_angle && height > min_climb_height) { current_state = State.Cling; Wall_Grab(); } // WALL 
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
        Quaternion rot = Quaternion.AngleAxis(slope_angle, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis((-slope_angle), Vector3.forward);
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot * Vector2.down));
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot2 * Vector2.down));
        #endregion

        #region WALL JUMP ANGLE
        Gizmos.color = Color.blue;
        float angle = wall_jump_angle * Mathf.RoundToInt(detection.x);
        Quaternion rot3 = Quaternion.AngleAxis(angle, Vector3.forward);
        Gizmos.DrawLine(transform.position, transform.position + (rot3 * Vector3.up));
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot3 * Vector2.down));

        #endregion

        #region PLAYER BOUNDS
        Gizmos.color = new Color(1.0f, 0.0f, 0.85f, 0.4f);
        Gizmos.DrawCube(transform.position + Player_Bounds.center, Player_Bounds.size);
        Gizmos.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        Gizmos.DrawWireCube(transform.position + Player_Bounds.center, Player_Bounds.size);
        #endregion
    }
}