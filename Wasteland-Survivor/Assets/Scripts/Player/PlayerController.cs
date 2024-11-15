using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Input variables
    private Vector2 moveInput;
    public float gravity = -9.81f;
    private bool bisGrounded;
    [SerializeField] private Camera mainCam;
    [SerializeField] Transform playerBodyPos;
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject sword;
    [SerializeField] GameObject currentWeapon;
    [SerializeField] GameObject Torch;

    private CharacterController controller;
    private FootstepController footstep;
    Animator animator;
    [SerializeField] float camOffset = 0.5f;
    [SerializeField] float crouchCamOffset = 0.25f;
    float xRotation = 0;
    float standingHeight,crouchingHeight;
    bool bIsCrouching = false;
    bool bIsJumping = false;
    bool bIsSprinting = false;
    Vector3 moveVelocity = new Vector3();
   [SerializeField]float minViewRotation = -90f;//Limit how much the player can look down
    [SerializeField] float walkSpeed = 20f;
    [SerializeField] float sprintSpeed = 40f;
    [SerializeField] float JumpSpeed = 2f;
    [SerializeField] float sprintDuration = 3f;
    private float maxSprintTime;
    private float maxJumps = 1;
    private float speed, crouchSpeed;
    

    //END PLAYER VAR
    //MouseSettings
    public float mouseSensitivity = 100f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        footstep = GetComponent<FootstepController>();
        speed = walkSpeed;
        controller.height = standingHeight;
        crouchingHeight = standingHeight / 2;
        crouchSpeed = speed / 2;
       
        Vector3 camPos = new Vector3(0f, camOffset, 0f);
        mainCam.transform.localPosition = camPos;
        if (currentWeapon.GetComponent<GunSystem>())
        {
            currentWeapon.GetComponent<GunSystem>().defaultFOV = mainCam.fieldOfView;
        }
        //sett all weapons to be inactive then make the current weapon only to be active
        DisableAll();
        currentWeapon.SetActive(true);
        maxSprintTime = sprintDuration;
        
    }

    private void OnLook(InputValue value)
    {
        //Get and clamp look values
        Vector2 look = value.Get<Vector2>();
        float lookX = look.x * mouseSensitivity * Time.deltaTime;
        float lookY = look.y * mouseSensitivity * Time.deltaTime;
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation,-90f,90f);
        //Rotate camera up
        mainCam.transform.localRotation = Quaternion.Euler(xRotation,0,0);
        //Rotate player entity
        transform.Rotate(Vector3.up * lookX);
    }
    private void OnFire(InputValue value)
    {
        if (currentWeapon.GetComponent<GunSystem>())
        {
            //Debug.Log("Calling Fire");
            currentWeapon.GetComponent<GunSystem>().Fire();
        }
        else if (currentWeapon.GetComponent<MeleeSystem>())
        {
            Debug.Log("Calling Melee");
            currentWeapon.GetComponent<MeleeSystem>().Attack();
        }
        
    }
    private void OnReload(InputValue value)
    {
        if (currentWeapon.GetComponent<GunSystem>())
        {
            currentWeapon.GetComponent<GunSystem>().Reload();
        }
    }
    private void OnThrow(InputValue value)
    {
        if(GetComponent<ThrowScript>() != null)
        {
            GetComponent<ThrowScript>().Throw();
        }
    }
    private void OnAim(InputValue value)
    {
        if (currentWeapon.GetComponent<GunSystem>())
        {

            currentWeapon.GetComponent<GunSystem>().SetAim();
        }
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }
    private void OnCrouch(InputValue value)
    {
        bIsCrouching = !bIsCrouching;
        //Debug.Log("Crouch Pressed");
    }
    
    private void OnJump(InputValue value)
    {
        if(bisGrounded)
        {
            bIsJumping = !bIsJumping;
        }
       
       
    }
    private void OnSprint(InputValue value)
    {
        bIsSprinting = !bIsSprinting;
    }
    private void OnTorch(InputValue value)
    {
        if (Torch.activeSelf)
        {
            Torch.SetActive(false);
        }
        else 
        {
            Torch.SetActive(true);
        }
    }
    public void DisableAll()
    {
        //I dont think we'll really need this outside of initilisation but might be useful in the future for cutscenes etc.
        pistol.SetActive(false);
        rifle.SetActive(false);
        sword.SetActive(false);
    }
    ///////////////////////
    /////WEAPON SWAPS/////
    /////////////////////
    private void OnPistol(InputValue value)
    {
        if(currentWeapon == pistol) { return; }
        currentWeapon = pistol;
        pistol.SetActive(true);
        sword.SetActive(false);
        rifle.SetActive(false);
    }
    private void OnSword(InputValue value)
    {
        if (currentWeapon == sword) { return; }
        currentWeapon = sword;
        pistol.SetActive(false);
        sword.SetActive(true);
        rifle.SetActive(false);
    }
    private void OnRifle(InputValue value)
    {
        if (currentWeapon == rifle) { return; }
        currentWeapon = rifle;
        pistol.SetActive(false);
        sword.SetActive(false);
        rifle.SetActive(true);
    }
    void Movement()
    {
        moveVelocity = transform.right * moveInput.x + transform.forward * moveInput.y;
        bisGrounded = controller.isGrounded;
        //Crouching
        if(bIsCrouching)
        {
            controller.height = crouchingHeight;
            speed = crouchSpeed;
            //Debug.Log("crouching");
            Vector3 camPos = new Vector3(0f, crouchCamOffset, 0f);
            mainCam.transform.localPosition = camPos;
        }
        else
        {
            controller.height = standingHeight;
            speed = walkSpeed;
            Vector3 camPos = new Vector3(0f, camOffset, 0f);
            mainCam.transform.localPosition = camPos;
        }
        //Gravity Implementation
        moveVelocity.y += gravity * Time.deltaTime;
        if (controller.isGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0;
            maxJumps = 0;
        }
        if(bIsJumping && maxJumps < 1)
        {
            moveVelocity.y += Mathf.Sqrt(JumpSpeed * -3.0f * gravity);
            bIsJumping = false;
            maxJumps++;
            
        }
        if(bIsSprinting && sprintDuration > 0)
        {
            speed = sprintSpeed;
            sprintDuration -= Time.deltaTime;
            sprintDuration = Mathf.Clamp(sprintDuration, 0f, maxSprintTime);
        }
        else 
        {
            speed = walkSpeed;
            sprintDuration += Time.deltaTime;
            sprintDuration = Mathf.Clamp(sprintDuration, 0f, maxSprintTime);
        }
        //FWD/Strafing movement
        moveVelocity.y += gravity * Time.deltaTime;
        controller.Move(moveVelocity * speed * Time.deltaTime);
       
       
    }
   public void ChangeMouseSensitivity(float newMouseSensitivity) { mouseSensitivity = newMouseSensitivity; }

    // ---------- //
    // ANIMATIONS //
    // ---------- //

    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";
    public const string ATTACK2 = "Attack 2";

    string currentAnimationState;
    public void ChangeAnimationState(string newState)
    {
       // Debug.Log("Change anim state called: " + newState);
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    void SetAnim()
    {
        
        if (moveVelocity.x == 0 && moveVelocity.z == 0)
        {
            ChangeAnimationState(IDLE);
           // Debug.Log("Changing Animation State to IDLE");
        }
        else
        {
            ChangeAnimationState(WALK);
         // Debug.Log(moveVelocity.ToString());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        SetAnim();
        if(controller.isGrounded && moveVelocity.magnitude > 0.5)
        {
            footstep.StartWalking();
        }
        else 
        {
           // Debug.Log("StopWalking");
           footstep.StopWalking();
        }
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
