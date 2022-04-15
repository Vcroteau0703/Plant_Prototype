using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    #region DECLARATIONS
    // SINGLETON INSTANCE //
    public static Player_Controller instance;

    //CLASS REFERENCES //
    public Controls controls;
    public Rigidbody rb;
    public Detection detection;
    public State_Controller state_controller;
    private Animator animator;
    public Player_Control_Settings settings;
    public Player_Control_Settings[] setting_presets;

    // DEBUG INFO //
    [Header("Debug")]
    public LayerMask walkable;
    public Vector2 direction;
    //public Vector2 touching;
    private float coyote_time;
    public float slide_speed = 0;
    public float aerial_time;
    public float target_gravity = -9.81f;
    public float friction = 0;

    // DETECTION //
    bool left, right, up, down;
    #endregion

    private void Awake()
    {
        instance = this;
        rb = TryGetComponent(out Rigidbody r) ? r : null;
        animator = TryGetComponent(out Animator a) ? a : null;
    }

    // Putting these here to allow me to call them within cutscenes and dialog
    public void EnableCling()
    {
        state_controller.Enable_State("Cling");
    }
    public void DisableCling()
    {
        state_controller.Disable_State("Cling");
    }
    public void EnableGlide()
    {
        state_controller.Enable_State("Glide");
    }
    public void OnEnable()
    {
        settings = setting_presets.Length > 0 ? setting_presets[0] : settings;
        controls = controls == null ? new Controls() : controls;
        Subscribe_Actions();
        controls.Player.Enable();
    }
    private void Start()
    {
    }
    private void FixedUpdate()
    {
        State_Control();
    }
    private void Update()
    {
        target_gravity = Physics.gravity.y;
        Animation_Driver();
        //Ground_Lock();
        friction = detection.collider.material.dynamicFriction;   
    }
    public void OnDisable()
    {
        controls.Player.Disable();
    }

    #region INPUT REQUESTS
    private void Request_Move(InputAction.CallbackContext context)
    {
        Vector2 temp = context.ReadValue<Vector2>();
        float x = Mathf.Abs(temp.x) > (settings.horizontal_deadzone / 100f) ? temp.x : 0.0f;
        float y = Mathf.Abs(temp.y) > (settings.vertical_deadzone / 100f) ? temp.y : 0.0f;
        direction = new Vector2(x, y);
        Flip((int)((direction.x % 1) + (direction.x / 1)));

        State state = state_controller.Get_Active_State();

        if(state.name != "Moving" && state.name != "Idle")
        {
            return; 
        }
        else if (direction != Vector2.zero){
            detection.collider.material.dynamicFriction = 0f;
            state_controller.Request_State("Moving");
        }
        else{
            detection.collider.material.dynamicFriction = 1.5f;
            state_controller.Request_State("Idle");
        }
    }
    private void Request_Jump(InputAction.CallbackContext context)
    {
        State state = state_controller.Get_Active_State();

        if (state.name == "Idle" || state.name == "Moving")
        {
            state_controller.Request_State("Jump");
        }       
        else if (state.name == "Cling")
        {
            state_controller.Request_State("Wall Jump");
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
    }  
    private int index = 0;
    private void Switch_Settings(InputAction.CallbackContext context)
    {
        index = setting_presets.Length - 1 > index ? index += 1 : 0;
        settings = settings == null ? setting_presets[index] : settings;
        Notification_System.Send_SystemNotify("Player control settings changed to " + settings.name, Color.blue);
    }
    #endregion

    #region STATE CONTROL

    /*
     * LOGIC:
     * 
     * [WALK/IDLE]
     * + Down?
     * + Direction.x 
     * 
     * [CLING]
     * + Ground?
     * + Left? or Right?
     * 
     * [SLIDE]
     * + Cling?
     * + Direction.y
     * 
     * [GLIDE]
     * + Holding Space?
     * + 
     * 
     */

    public void State_Control()
    {
        left = detection.Get_Detection("Left").target;
        right = detection.Get_Detection("Right").target;
        down = detection.Get_Detection("Down").target;

        float slope = detection.Get_Slope_Angle();
        float wall = detection.Get_Wall_Angle(settings.Cling_Threshold);


        State active = state_controller.Get_Active_State();

        Debug.Log(active.name);


        if((left || right) && aerial_time > 0.25f) // Wall
        {        
            if(wall < 30 && wall >= 0 && state_controller.Get_State("Cling").enabled)
            {               
                if (direction.y < 0 && slope == -1)
                {
                    state_controller.Request_State("Slide");            
                    return;
                }
                else
                {
                    state_controller.Request_State("Cling");
                    return;
                }
            }            
        } 
     
        if(detection.Is_Detecting() == false || slope == -1) //Air
        {
            State glide = state_controller.Get_State("Gliding");
            if (settings.Jump.phase == Jump.State.Canceled &&
                settings.Wall_Jump.phase == Jump.State.Canceled &&
                controls.Player.Glide.phase == InputActionPhase.Performed){
                state_controller.Request_State("Gliding");
                return;
            }          
            state_controller.Request_State("Aerial");
            return;                     
        }
        if (down) // Ground
        {
            
            if (direction.x != 0)
            {
                aerial_time = 0;
                state_controller.Request_State("Moving");
                return;
            }
            else
            {
                aerial_time = 0;
                state_controller.Request_State("Idle");
                return;
            }
        }
    }

    #endregion

    #region MOVEMENT
    public void Move()
    {        
        rb.isKinematic = false;
        detection.collider.material.dynamicFriction = 0f;
        float ground_clamp = -0.75f;

        Vector2 pos = detection.collider.transform.position;
        Vector2 dir = detection.Get_Slope_Direction();

        float speed = direction.x * settings.Move_Speed;
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.Acceleration) * direction.x;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -Mathf.Abs(force), Mathf.Abs(force)), rb.velocity.y);

        Debug.DrawLine(pos, pos + (dir * force).normalized, Color.red);

        rb.AddForceAtPosition(dir * force, pos, ForceMode.Force);
        float friction_force = Mathf.Abs(direction.x) < settings.horizontal_deadzone ? Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(settings.Friction)) : 0;
        friction_force *= Mathf.Sign(rb.velocity.x);
        rb.AddForce(dir * -friction_force, ForceMode.Impulse);
    }
    public void Cling()
    {
        //CLING
        rb.isKinematic = settings.Wall_Jump.phase != Jump.State.Started ? true : false;
        foreach (Detection_Cast c in detection.Get_All_Detections())
        {
            if (c.name == "Left")
            {
                Vector2 dir = -detection.transform.right;
                rb.AddForce(dir * settings.Cling_Power, ForceMode.Force);
                Flip(-1);
            }
            else if (c.name == "Right")
            {
                Vector2 dir = detection.transform.right;
                Debug.Log(dir);
                rb.AddForce(dir * settings.Cling_Power, ForceMode.Force);
                Flip(1);
            }
        }


        // AUTO SLIDE + FRICTION
        transform.position += Vector3.down * slide_speed * Time.deltaTime;
        if (slide_speed > 0) { slide_speed -= Time.deltaTime * settings.Slide_Friction; }
        else { slide_speed = 0; }
    }
    public void Eject()
    {
        rb.isKinematic = false;
        //WALL EJECT
        if (detection.Get_Detection("Right").target != null && direction.x < 0)
        {
            rb.AddForce(settings.Eject_Power * Vector3.left, ForceMode.Impulse);
        }
        else if(detection.Get_Detection("Left").target != null && direction.x > 0)
        {
            rb.AddForce(settings.Eject_Power * Vector3.right, ForceMode.Impulse);
        }
    }
    public void Aerial()
    {
        
        rb.isKinematic = false;
        detection.collider.material.dynamicFriction = 0;
        aerial_time += Time.deltaTime;
        slide_speed = rb.velocity.y < -settings.Slide_Threshhold && Mathf.Abs(rb.velocity.y) > slide_speed ? Mathf.Clamp(Mathf.Abs(rb.velocity.y), 0, settings.Max_Slide_Speed) : slide_speed;
        float speed = direction.x * settings.Air_Speed;
        if (left) { speed = Mathf.Clamp(speed, 0, settings.Air_Speed); }
        else if (right) { speed = Mathf.Clamp(speed, -settings.Air_Speed, 0); }
        float diff = speed - rb.velocity.x;
        float force = Mathf.Pow(Mathf.Abs(diff), settings.Air_Accel) * direction.x;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -settings.Air_Speed, settings.Air_Speed), rb.velocity.y);
        rb.AddForce(force * settings.Air_Control * Vector3.right, ForceMode.Acceleration);
        
        if (rb.velocity.y < settings.Jump.power && rb.velocity.y > settings.Fall_Speed/2)
        {
            Physics.gravity = new Vector3(Physics.gravity.x, settings.Fall_Speed, Physics.gravity.z);
        }
        else
        {
            Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
        }
    }
    public void Ceiling()
    {
        rb.isKinematic = false;
        rb.AddForce(-transform.up * settings.Bonk_Power, ForceMode.Impulse);
    }
    public void Glide()
    {
        Aerial();
        rb.isKinematic = false;
        rb.AddForce(transform.up * -Physics.gravity.y / (10 / settings.Glide_Power), ForceMode.Force);
        rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -1 / settings.Glide_Power, float.PositiveInfinity), rb.velocity.z);
    }
    public void Slide()
    {
        Cling();
        Vector3 dir = detection.Get_Wall_Direction();
        Debug.DrawLine(transform.position, transform.position + dir * slide_speed, Color.green);
        if (direction.y < 0 && detection.Is_Detecting())
        {
            float a = slide_speed + Time.deltaTime * settings.Slide_Accel;
            slide_speed = Mathf.Clamp(a, 0, settings.Max_Slide_Speed);
            transform.position += dir * slide_speed * Time.deltaTime;
            return;
        }
    }
    public void Wall_Jump()
    {
        rb.isKinematic = false;
        Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
        StartCoroutine(Jump_02(settings.Wall_Jump, 0));
    }
    public void Vertical_Jump()
    {
        rb.isKinematic = false;
        Physics.gravity = new Vector3(Physics.gravity.x, -9.81f, Physics.gravity.z);
        StartCoroutine(Jump_01(settings.Jump, 0));
    }
    public IEnumerator Jump_01(Jump jump, float time)
    {       
        jump.phase = Jump.State.Started;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(jump.power * Vector3.up, ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {            
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.power / (1 / jump.floatiness) * -Physics.gravity.y);
            if (force < 0.1f)
            {
                jump.phase = jump.phase != Jump.State.Waiting ? Jump.State.Canceled : Jump.State.Waiting;
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

        Detection_Cast right = detection.Get_Detection("Right"), left = detection.Get_Detection("Left");

        float angle = 0;

        if (right.target != null)
        {
            angle = settings.Wall_Jump.angle;
        }
        else if (left.target != null)
        {
            angle = settings.Wall_Jump.angle * -1;
        }

        Vector2 dir = Quaternion.AngleAxis(angle, Vector3.forward) * -Vector2.up;

        rb.AddForce(jump.power * dir, ForceMode.Impulse);
        while (jump.phase == Jump.State.Started)
        {
            time += Time.deltaTime;
            float force = -Mathf.Pow(time, 2) + (jump.power * 0.90f / (1 / jump.floatiness));
            if (force < 0.1f)
            {
                jump.phase = jump.phase != Jump.State.Waiting ? Jump.State.Canceled : Jump.State.Waiting;
                break;
            }
            rb.AddForce(force * Vector3.up, ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
        jump.phase = jump.phase != Jump.State.Waiting ? Jump.State.Canceled : Jump.State.Waiting;
    }
    public IEnumerator Jump_02_Buffer()
    {
        state_controller.Disable_State("Aerial");
        yield return new WaitForSeconds(settings.Wall_Jump.buffer);
        state_controller.Enable_State("Aerial");
    }
    public void Reset_Jump(Jump jump)
    {
        jump.phase = Jump.State.Waiting;
    }
    public void Cancel_Jump(Jump jump)
    {
        jump.phase = Jump.State.Canceled;
    }
    #endregion

    #region DEPRECATED FUNCTIONS
    private void Ground_Lock()
    {
        string state = state_controller.Get_Active_State().name;
        if(state != "Moving" && state != "Idle") { return; }
        RaycastHit hit;
        Vector2 dir = Quaternion.AngleAxis(-90, Vector3.forward) * detection.Get_Slope_Direction();
        Vector2 pos = detection.transform.position;
        Debug.DrawRay(pos, dir * 1.5f, Color.red, 1.5f);
        if (Physics.Raycast(pos, dir * 1.5f, out hit, 1.5f))
        {
            if(hit.distance < 1.2f) { return; }          
            transform.position += (hit.distance - detection.collider.bounds.extents.y) * Vector3.down;
            Debug.Log(hit.distance);
        }
    }
    #endregion

    #region UTILITY
    private void Subscribe_Actions()
    {
        controls.Player.Move.performed += Request_Move;
        controls.Player.Move.canceled += Request_Move;
        controls.Player.Jump.performed += Request_Jump;
        controls.Player.Jump.canceled += Request_Cancel_Jump;
        controls.Player.Switch_Controls.performed += Switch_Settings;
    }
    private void Animation_Driver()
    {
        animator.SetInteger("DIR_X", (int)direction.x);
        animator.SetInteger("DIR_Y", (int)direction.y);
        animator.SetFloat("VELOCITY_Y", Mathf.Round(rb.velocity.y));
        animator.SetFloat("VELOCITY_X", Mathf.Round(rb.velocity.x));
        animator.SetFloat("WALK_SPEED", rb.velocity.x * direction.x);

        animator.SetBool("JUMP_01", state_controller.Get_Active_State().name == "Jump");
        animator.SetBool("JUMP_02", state_controller.Get_Active_State().name == "Wall Jump");
        animator.SetBool("MOVING", state_controller.Get_Active_State().name == "Moving");
        animator.SetBool("IDLE", state_controller.Get_Active_State().name == "Idle");
        animator.SetBool("AERIAL", state_controller.Get_Active_State().name == "Aerial");
        animator.SetBool("CLING", state_controller.Get_Active_State().name == "Cling");
        animator.SetBool("GLIDE", state_controller.Get_Active_State().name == "Gliding");
        //animator.SetBool("CEILING", state_controller.Get_Active_State().name == "Aerial");
        animator.SetBool("SLIDE", state_controller.Get_Active_State().name == "Slide");
    }
    public void Flip(int direction)
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
    public void Flip()
    {
        if(direction.x != 0) { return; }
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    #endregion
}

#region CLASSES
[System.Serializable]
public class Jump
{
    public float power;
    public float floatiness;
    public float angle;
    public float buffer;

    public enum State { Waiting = default, Canceled, Started, Performing }
    public State phase;
}
#endregion