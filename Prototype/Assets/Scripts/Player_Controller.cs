using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    //Class References
    private Controls controls;
    public Rigidbody rb;
    public CapsuleCollider col;

    [Header("Movement Settings")]
    public float move_speed = 1.0f;
    public float move_smoothing = 0.5f;
    public float jump_power = 1.0f;
    public float force, time, floatiness;

    private Vector2 direction;
    public bool grounded;

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

    private void Request_Movement(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
    }

    private void Movement()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(direction.x * move_speed, rb.velocity.y, 0), move_smoothing);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (grounded == true)
        {
            StartCoroutine(Jump());         
        }
    }

    private IEnumerator Jump()
    {
        Debug.Log("Jump");
        time = 0;
        while (time != 10f)
        {
            time += Time.deltaTime;
            force = Mathf.Pow(-time, 2)/ floatiness * (Mathf.Pow(time, 2) / floatiness) + jump_power;
            rb.AddForce(new Vector3(0,force,0), ForceMode.Acceleration);
            yield return new WaitForEndOfFrame();
            if(force < 0) { break; }
        }
    }

    private bool Ground_Check()
    {
        Vector3 pos = (transform.position - (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.down * 0.02f));
        Collider[] floor = Physics.OverlapBox(pos, new Vector3(0.5f, 0.01f, 0.5f));
        if(floor.Length > 0) { return true; }
        else{return false;}
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        grounded = Ground_Check();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnDrawGizmos()
    {
        if (grounded)
        {
            Gizmos.color = Color.blue;
            Vector3 pos = (transform.position - (Vector3.up * (col.bounds.size.y / 2)) + (Vector3.down * 0.02f));
            Gizmos.DrawWireCube(pos, new Vector3(0.5f, 0.01f, 0.5f));
        }
    }
}
