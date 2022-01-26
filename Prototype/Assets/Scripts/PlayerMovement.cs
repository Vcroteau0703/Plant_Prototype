using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class PlayerMovement : MonoBehaviour
{

    private Controls controls;

    // movement variables
    public float movementSpeed = 4.5f;
    private Vector2 move;
    public bool isMoving;

    private CharacterController characterController;
    public Vector3 velocity;
    public Vector3 movement;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    public DialogueRunner dialog;

    //public Transform groundCheck;
    //public float groundDistance = 0.4f;
    //public LayerMask groundMask;

    public bool isGrounded;
    public bool onWall;

    private RaycastHit vision;
    public float rayLength;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private void Awake()
    {
        controls = new Controls();
        characterController = GetComponent<CharacterController>();

        controls.Player.Jump.performed += ctx => Jump();

        controls.Player.Interact.performed += ctx => Interact();

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = characterController.isGrounded;

        //Vector3 fwd = transform.TransformDirection(Vector3.forward);

        //movement
        if (transform.hasChanged)
        {
            isMoving = true;
            transform.hasChanged = false;
        }
        else
        {
            isMoving = false;
        }

        move = controls.Player.Move.ReadValue<Vector2>();

        //gravity and movement 
        if (!isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity += Vector3.up * gravity * (fallMultiplier - 1) * Time.deltaTime;
            }
            velocity.y += gravity * Time.deltaTime;
        }
        movement = move.y * transform.forward + (move.x * transform.right);

        characterController.Move(movement * movementSpeed * Time.deltaTime);

        characterController.Move(velocity * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Hazard":
                Debug.Log("die");
                SceneManager.LoadScene(0);
                break;
            case "Pickup":
                other.transform.SetParent(transform);
                other.transform.localPosition = new Vector3(0f, 1.25f, 0f);
                other.gameObject.GetComponent<BoxCollider>().isTrigger = false;
                break;
            default:
                Debug.Log("no tag with any associated behaviors");
                break;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.transform.tag == "Wall")
        {
            onWall = true;
            velocity.y = 0f;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.tag == "Wall")
    //    {
    //        StopCoroutine(WallHang());
    //        onWall = false;
    //    }
    //}

    void Jump()
    {
        if (isGrounded || onWall)
        {
            velocity.y = jumpHeight;
            onWall = false;
        }
    }

    //IEnumerator WallHang()
    //{
    //    onWall = true;
    //    velocity.y = 0f;
    //    yield return new WaitForSeconds(1);
    //    onWall = false;
    //}

    public void Interact()
    {
        Debug.DrawRay(transform.position, transform.right * rayLength, Color.red, 0.5f);

        if(Physics.Raycast(transform.position, transform.right, out vision, rayLength))
        {
            if(vision.transform.tag == "Character")
            {
                if(transform.childCount > 0)
                {
                    dialog.StartDialogue("GotSeedling");
                    transform.GetChild(0).SetParent(vision.transform);
                    vision.transform.GetChild(0).position = new Vector3(6.51f, 5.192f, 0f);
                }
                else
                {
                    dialog.StartDialogue("Start");
                }
            }
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}

