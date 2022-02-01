using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    //Class References
    private Controls controls;
    private Rigidbody rb;
    private CapsuleCollider col;


    [Header("Movement Settings")]
    public float move_speed = 1.0f;
    public float move_smoothing = 0.5f;
    public float jump_power = 1.0f;
    public float floatiness, weight;
    public float air_control = 1.0f;
    public float max_air_speed = 10.0f;
    public float wall_jump_power = 10.0f;
    public float wall_grab_strength = 20.0f;
    public float wall_slide_speed = 5.0f;

    private Vector2 direction;
    private bool grounded;
    private bool[] wall = new bool[2];
    private bool ceiling;

    private void Awake()
    {
        rb = TryGetComponent(out Rigidbody r) ? r : null;
        col = TryGetComponent(out CapsuleCollider c) ? c : null;
        controls = controls == null ? new Controls() : controls;
        controls.Player.Move.performed += Request_Movement;
        controls.Player.Move.canceled += Request_Movement;
        controls.Player.Jump.performed += Jump;
        controls.Player.Enable();
    }

    #region MOVEMENT/AIR CONTROL
    private void Request_Movement(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    private void Movement()
    {
        if(grounded == false) 
        {
            float air_speed = Mathf.Clamp(Mathf.Lerp(rb.velocity.x, rb.velocity.x + direction.x * move_speed, air_control), -max_air_speed, max_air_speed);
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(air_speed, rb.velocity.y, 0), move_smoothing);
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(direction.x * move_speed, rb.velocity.y, 0), move_smoothing);
        }
    }
    #endregion

    #region JUMP
    private void Jump(InputAction.CallbackContext context)
    {
        if (grounded == true){StartCoroutine(Jump()); }
        else if (wall[0] || wall[1]) { StartCoroutine(Wall_Jump()); }
    }

    private IEnumerator Jump()
    {
        Debug.Log("Jump");        
        float force;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jump_power, ForceMode.Force);
        yield return new WaitForEndOfFrame();
        grounded = false;
        float time = 0;
        while (time == 0 || !grounded)
        {
            time += Time.deltaTime;
            force = (-Mathf.Pow(time, 2) / (1/weight*10)) + floatiness;
            rb.AddForce(new Vector3(0,force,0), ForceMode.Acceleration);
            Debug.Log("Force");
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region WALL JUMP

    public IEnumerator Wall_Jump()
    {
        Debug.Log("Wall Jump");
        grounded = false;
        //float force;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(controls.Player.Pointer.ReadValue<Vector2>());
        Vector2 dir =  (mouse - (Vector2)transform.position).normalized;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(dir * wall_jump_power, ForceMode.Force);
        yield return new WaitForEndOfFrame();
        //float time = 0;
        //while (time == 0 || !grounded)
        //{
        //    time += Time.deltaTime;
        //    force = (-Mathf.Pow(time, 2) / (1 / weight * 10)) + floatiness;
        //    rb.AddForce(new Vector3(0, force, 0), ForceMode.Acceleration);
        //    yield return new WaitForEndOfFrame();
        //    if (grounded || wall[0] || wall[1] || ceiling) { break; }
        //}      
    }

    public void Wall_Grab()
    {
        controls.Player.Move.Disable();
        if (wall[0])
        {
            rb.AddForce(Vector2.left * wall_grab_strength);
            Wall_Slide();
        }
        else if(wall[1])
        {
            rb.AddForce(Vector2.right * wall_grab_strength);
            Wall_Slide();
        }
    }

    public void Wall_Slide()
    {
        rb.AddForce(Vector3.down * wall_slide_speed, ForceMode.Acceleration);
    }
    #endregion

    #region UTILITY

    public float delay = 0;
    private void Ground_Check()
    {
        Vector3 pos = (transform.position - (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.down * 0.02f));
        Collider[] hit = Physics.OverlapBox(pos, new Vector3(0.2f, 0.01f, 0.2f));
        if(hit.Length > 0) { grounded = true; delay = 0.0f; return; }
        if (delay < 0.15f && rb.velocity.y < 0.0f) { delay += Time.deltaTime; return; }
        grounded = false;
    }

    private void Ceiling_Check()
    {
        Vector3 pos = (transform.position + (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.up * 0.02f));
        Collider[] hit = Physics.OverlapBox(pos, new Vector3(0.2f, 0.01f, 0.2f));
        ceiling = hit.Length > 0 ? true : false;
    }

    private void Wall_Check()
    {
        Vector3 posA = (transform.position - (Vector3.left * (col.bounds.size.x / 2)) + (Vector3.left * 0.7f));
        Collider[] hit_left = Physics.OverlapBox(posA, new Vector3(0.01f, 0.2f, 0.2f));        
        wall[0] = hit_left.Length > 0 ? true : false;
        if (wall[0]) { Wall_Grab(); }

        Vector3 posB = (transform.position - (Vector3.right * (col.bounds.size.x / 2)) + (Vector3.right * 0.7f));
        Collider[] hit_right = Physics.OverlapBox(posB, new Vector3(0.01f, 0.2f, 0.2f));
        wall[1] = hit_right.Length > 0 ? true : false;
        if (wall[1]) { Wall_Grab(); }
        else if(!wall[0] && !wall[1]) { controls.Player.Move.Enable(); }
    }
    #endregion

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
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
        if (controls != null)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(controls.Player.Pointer.ReadValue<Vector2>());
            mouse.z = transform.position.z;
            Gizmos.DrawSphere(mouse, 0.2f);
            Gizmos.DrawLine(transform.position, mouse);
        }
      
        if (grounded)
        {
            Vector3 pos = (transform.position - (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.down * 0.02f));
            Gizmos.DrawWireCube(pos, new Vector3(col.bounds.size.x / 2, 0.01f, 0.2f) * 2);
        }
        if (wall[0])
        {         
            Vector3 pos = (transform.position - (Vector3.left * (col.bounds.size.x / 2)) + (Vector3.left * 0.7f));
            Gizmos.DrawWireCube(pos, new Vector3(0.01f, col.bounds.size.y/2, 0.2f) * 2);
        }
        if (wall[1])
        {
            Vector3 pos = (transform.position - (Vector3.right * (col.bounds.size.x / 2)) + (Vector3.right * 0.7f));
            Gizmos.DrawWireCube(pos, new Vector3(0.01f, col.bounds.size.y/2, 0.2f) * 2);
        }
        if (ceiling)
        {
            Vector3 pos = (transform.position + (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.up * 0.02f));
            Gizmos.DrawWireCube(pos, new Vector3(col.bounds.size.x / 2, 0.01f, 0.2f) * 2);
        }
    }
}
