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
    public float force, time, floatiness, weight;
    public float air_control = 1.0f;
    public float max_air_speed = 10f;

    private Vector2 direction;
    private bool grounded;
    private bool[] wall = new bool[2];

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
        if (grounded == true)
        {
            StartCoroutine(Jump());         
        }
    }

    private IEnumerator Jump()
    {
        rb.AddForce(Vector3.up * jump_power, ForceMode.Force);
        time = 0;
        while (time != 10f)
        {
            time += Time.deltaTime;
            force = (-Mathf.Pow(time, 2) / (1/weight*10)) + floatiness;
            rb.AddForce(new Vector3(0,force,0), ForceMode.Acceleration);
            yield return new WaitForEndOfFrame();
            if(grounded) { force = 0; break; }
        }
    }
    #endregion

    #region WALL JUMP

    public void Wall_Jump()
    {

    }

    #endregion

    #region UTILITY
    private void Ground_Check()
    {
        Vector3 pos = (transform.position - (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.down * 0.02f));
        Collider[] floor = Physics.OverlapBox(pos, new Vector3(0.2f, 0.01f, 0.2f));
        grounded = floor.Length > 0 ? true : false;
    }

    private void Wall_Check()
    {
        Vector3 posA = (transform.position - (Vector3.left * (col.bounds.size.x / 2)) + (Vector3.left * 0.7f));
        Collider[] wall_left = Physics.OverlapBox(posA, new Vector3(0.01f, 0.2f, 0.2f));
        wall[0] = wall_left.Length > 0 ? true : false;

        Vector3 posB = (transform.position - (Vector3.right * (col.bounds.size.x / 2)) + (Vector3.right * 0.7f));
        Collider[] wall_right = Physics.OverlapBox(posB, new Vector3(0.01f, 0.2f, 0.2f));
        wall[1] = wall_right.Length > 0 ? true : false;
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
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
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
    }
}
