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
    [Header("Input Settings")]
    [Range(0f, 100f)] public float horizontal_deadzone = 20f;
    [Range(0f, 100f)] public float vertical_deadzone = 20f;

    private Vector2 direction;
    private bool grounded, ceiling, isControlling = true;
    private bool[] wall = new bool[2];
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
        animator.SetFloat("Speed_X", Mathf.Abs((int)((direction.x % 1) + (direction.x / 1))));
    }

    private void Movement()
    {      
        animator.SetFloat("Speed_Y", rb.velocity.y);
        Flip((int)((direction.x % 1) + (direction.x / 1)));
        if (!isControlling) { return; }
        float speed;
        if (grounded == false)
        {
            if ((wall[0] || wall[1]) && direction.x != 0)
            {
                speed = direction.x * move_speed * wall_release_strength / 100;
                rb.velocity = new Vector3(speed, rb.velocity.y, 0);
            }
            else
            {
                speed = Mathf.Clamp(Mathf.Lerp(rb.velocity.x, rb.velocity.x + direction.x * move_speed, air_control), -max_air_speed, max_air_speed);
                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(speed, rb.velocity.y, 0), move_smoothing);
            }
        }
        else
        {
            speed = direction.x * move_speed;
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(speed, rb.velocity.y, 0), move_smoothing);
        }
        if ((wall[0] || wall[1]) && direction.y < 0) { Wall_Slide(wall_slide_speed); }

    }
    #endregion

    #region JUMP
    private void Request_Jump(InputAction.CallbackContext context)
    {
        if (grounded == true){ StartCoroutine(Jump()); animator.SetTrigger("Jump"); }
        else if (wall[0] || wall[1]) { StartCoroutine(Wall_Jump()); animator.SetTrigger("Wall_Jump");}
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
            //force = Mathf.Clamp(force, -max_fall_speed, float.PositiveInfinity);
            if (rb.velocity.y > -max_fall_speed) { rb.AddForce(Vector3.up * force, ForceMode.Acceleration); };
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -max_fall_speed, float.PositiveInfinity), rb.velocity.z);
            yield return new WaitForEndOfFrame();
            if(grounded && time > 0.1f || wall[0] || wall[1] || ceiling) { break; }
        }
    }
    #endregion

    #region WALL JUMP
    public IEnumerator Wall_Jump()
    {
        isControlling = false;
        grounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Quaternion rotA = Quaternion.AngleAxis(wall_jump_angle, Vector3.forward);
        Quaternion rotB = Quaternion.AngleAxis(-wall_jump_angle, Vector3.forward);
        if (wall[0]){rb.AddForce(rotA * Vector3.right * wall_jump_power, ForceMode.Force);}
        else if (wall[1]){rb.AddForce(-(rotB * Vector3.right) * wall_jump_power, ForceMode.Force);}
        yield return new WaitForSeconds(0.2f);       
        isControlling = true;

        float force;
        float time = 0;
        while (time >= 0)
        {
            time += Time.deltaTime;
            force = -Mathf.Pow(time, 2) / (1 / weight * 10) + floatiness;
            if (controls.Player.Jump.phase == InputActionPhase.Waiting) { force = -Mathf.Abs(force) * downforce; }
            //force = Mathf.Clamp(force, -max_fall_speed, float.PositiveInfinity);
            if (rb.velocity.y > -max_fall_speed) { rb.AddForce(Vector3.up * force, ForceMode.Acceleration); };
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -max_fall_speed, float.PositiveInfinity), rb.velocity.z);
            yield return new WaitForEndOfFrame();
            if (grounded && time > 0.1f || wall[0] || wall[1] || ceiling) { break; }
        }
    }
    private float slide_time = 0;
    public void Wall_Grab(Vector3 dir)
    {
        animator.SetBool("Cling", true);
        //rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, float.NegativeInfinity, 1), rb.velocity.z);
        if (wall[0]){ rb.velocity = new Vector3(0, rb.velocity.y, 0); }
        else if(wall[1]){rb.AddForce(Vector3.right * wall_grab_strength, ForceMode.Force);}
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
    private float delay = 0;
    private void Ground_Check()
    {
        Vector3 pos = (transform.position - (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.down * 0.1f));
        Collider[] hit = Physics.OverlapBox(pos, new Vector3(col.bounds.size.x / 2 - 0.1f, 0.1f, 0.2f), Quaternion.identity, walkable);
        if(hit.Length > 0 && !wall[0] && !wall[1]) { Debug.Log("We hit: " + hit[0].name); grounded = true; delay = 0.0f; animator.SetBool("Ground", true); return; }
        if (delay < coyote_jump_delay && rb.velocity.y < 0.0f) { delay += Time.deltaTime; return; }
        grounded = false;
        animator.SetBool("Ground", false);
    }
    private void Ceiling_Check()
    {
        Vector3 pos = (transform.position + (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.up * 0.1f));
        Collider[] hit = Physics.OverlapBox(pos, new Vector3(0.2f, 0.1f, 0.2f), Quaternion.identity, walkable);
        ceiling = hit.Length > 0 ? true : false;
    }
    private void Wall_Check()
    {
        Vector3 posA = (transform.position - (Vector3.left * -(col.bounds.size.x / 2)) + (Vector3.left * 0.02f));
        Collider[] hit_left = Physics.OverlapBox(posA, new Vector3(0.1f, 0.5f, 0.2f), Quaternion.identity, walkable);        
        wall[0] = hit_left.Length > 0 ? true : false;
        if (wall[0] && !grounded) {Wall_Grab(transform.position + Vector3.left); rb.useGravity = false; Flip(-1);}

        Vector3 posB = (transform.position - (Vector3.right * -(col.bounds.size.x / 2)) + (Vector3.right * 0.02f));
        Collider[] hit_right = Physics.OverlapBox(posB, new Vector3(0.1f, 0.5f, 0.2f), Quaternion.identity, walkable);
        wall[1] = hit_right.Length > 0 ? true : false;
        if (wall[1] && !grounded) {Wall_Grab(transform.position + Vector3.right); rb.useGravity = false; Flip(1);}

        if(!wall[0] && !wall[1]) { rb.useGravity = true; slide_time = 0; animator.SetBool("Cling", false);}
    }
    private void Flip(int direction)
    {
        switch (direction) {
            default:
                if (wall[0]) { Flip(1); }
                else if (wall[1]) { Flip(-1); }
                break;
            case 1:
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;
            case -1:
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                break;
        }
    }
    #endregion

    private void FixedUpdate()
    {
        Movement();
        Ground_Check();
        Wall_Check();
        Ceiling_Check();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //#region WALL JUMP DIRECTIONS
        //Quaternion rot = Quaternion.AngleAxis(wall_jump_angle, Vector3.forward);
        //Quaternion rot2 = Quaternion.AngleAxis(-wall_jump_angle, Vector3.forward);
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)(rot * Vector2.right));
        //Gizmos.DrawLine(transform.position, (Vector2)transform.position - (Vector2)(rot * Vector2.left));
        //#endregion

        #region SURFACE INDICATORS
        if (grounded)
        {
            Vector3 pos = (transform.position - (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.down * 0.1f));
            Gizmos.DrawWireCube(pos, new Vector3(col.bounds.size.x / 2 - 0.1f, 0.1f, 0.2f) * 2);
        }
        if (wall[0])
        {         
            Vector3 pos = (transform.position - (Vector3.left * -(col.bounds.size.x / 2)) + (Vector3.left * 0.1f));
            Gizmos.DrawWireCube(pos, new Vector3(0.1f, 0.5f, 0.2f) * 2);
        }
        if (wall[1])
        {
            Vector3 pos = (transform.position - (Vector3.right * -(col.bounds.size.x / 2)) + (Vector3.right * 0.1f));
            Gizmos.DrawWireCube(pos, new Vector3(0.1f, 0.5f, 0.2f) * 2);
        }
        if (ceiling)
        {
            Vector3 pos = (transform.position + (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.up * 0.1f));
            Gizmos.DrawWireCube(pos, new Vector3(col.bounds.size.x / 2, 0.1f, 0.2f) * 2);
        }
        #endregion
    }
}