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
    [SerializeField]Transform playerBodyPos;
    
    private CharacterController controller;
    
    float xRotation = 0;
    [SerializeField] float startingHealth = 100;
    [SerializeField]float minViewRotation = -90f;//Limit how much the player can look down
    [SerializeField] float walkSpeed = 20f;
    [SerializeField] float JumpSpeed = 5f;
    private float speed;
    private float health;

    //END PLAYER VAR
    //MouseSettings
    public float mouseSensitivity = 100f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        speed = walkSpeed;
        health = startingHealth;
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

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

    }
    private void OnCrouch(InputValue value)
    {

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
    
    public void ChangeHealth(float hp)
    {
        //changes the amount of health the player currently has by the parameter value. Positive increases health, negative decreases. (e.g 100 + -25 = 75)
        health += hp;
        Debug.Log(health);
        //check for 0  health then die or something I guess 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }
}
