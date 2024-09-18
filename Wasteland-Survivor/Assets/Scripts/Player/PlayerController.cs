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
    
    private CharacterController controller;
    
    float xRotation = 0;
    float standingHeight,crouchingHeight;
    bool bIsCrouching = false;
    
    [SerializeField]float minViewRotation = -90f;//Limit how much the player can look down
    [SerializeField] float walkSpeed = 20f;
    [SerializeField] float JumpSpeed = 5f;
    private float speed, crouchSpeed;
    

    //END PLAYER VAR
    //MouseSettings
    public float mouseSensitivity = 100f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = walkSpeed;
        controller.height = standingHeight;
        crouchingHeight = standingHeight / 2;
        crouchSpeed = speed / 2;
    }
    private void OnLook(InputValue value)
    {
        //Get and clamp look values
        Vector2 look = value.Get<Vector2>();
        float lookX = look.x * mouseSensitivity * Time.deltaTime;
        float lookY = look.y * mouseSensitivity * Time.deltaTime;
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation,-90f,minViewRotation);
        //Rotate camera up
        mainCam.transform.localRotation = Quaternion.Euler(xRotation,0,0);
        //Rotate player entity
        transform.Rotate(Vector3.up * lookX);
    }
    private void OnFire(InputValue value)
    {
        if (GetComponent<GunSystem>())
        {
            Debug.Log("Calling Fire");
            GetComponent<GunSystem>().Fire();
        }
        
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }
    private void OnCrouch(InputValue value)
    {
        bIsCrouching = !bIsCrouching;
        Debug.Log("Crouch Pressed");
    }
    
    private void OnJump(InputValue value)
    {
        if(controller.isGrounded)
        {
            //Vector3 jumpVelocity.y += Mathf.Sqrt(JumpSpeed * -2f * gravity);
        }
       // controller.Move(jumpVelocity * Time.deltaTime);
    }
    void Movement()
    {
        Vector3 moveVelocity = transform.right * moveInput.x + transform.forward * moveInput.y;
        bisGrounded = controller.isGrounded;
        //Crouching
        if(bIsCrouching)
        {
            controller.height = crouchingHeight;
            speed = crouchSpeed;
            //Debug.Log("crouching");

        }
        else
        {
            controller.height = standingHeight;
            speed = walkSpeed;
            //Debug.Log("not crouching");
        }
        //Gravity Implementation
        moveVelocity.y += gravity * Time.deltaTime;
        if (controller.isGrounded)
        {
            moveVelocity.y = 0;
        }
        else
        {
            moveVelocity.y += gravity * Time.deltaTime;
        }
        //FWD/Strafing movement
      
        controller.Move(moveVelocity * speed * Time.deltaTime);
       
       
    }
   public void ChangeMouseSensitivity(float newMouseSensitivity) { mouseSensitivity = newMouseSensitivity; }
    
   

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
}
