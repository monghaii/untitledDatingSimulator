using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mixpanel;

public class FirstPersonController : MonoBehaviour
{
    public CharacterController controller;

    public enum MoveState
    {
        Walk,
        Sprint,
        Crouch,
        InAir
    }
    
    [Header("Gravity")] 
    public float gravity;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Movement")] 
    public MoveState moveState;
    public float walkSpeed = 12f;
    public float crouchSpeed = 10f;
    public float crouchScale;
    private float startScale;
    public float sprintSpeed = 15f;
    
    private float speed;
    private Vector3 velocity;
    private Vector3 move;
    public float jumpHeight;


    void Start()
    {
        startScale = transform.localScale.y;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(GameManager.isGamePaused)
        {
            return;
        }

        // calculate updates
        GravityUpdate();
        Movement();
        Jump();
        Crouch();
        CheckState();
        
        // apply updates to player controller
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);
    }

    void CheckState()
    {
        if (isGrounded && (Input.GetKey(KeyCode.LeftShift)))
        { 
            // SPRINTING (hold)
            moveState = MoveState.Sprint;
            speed = sprintSpeed;
        }
        else if (isGrounded && Input.GetKeyDown(KeyCode.LeftControl))
        {
            // CROUCHING (hold)
            moveState = MoveState.Crouch;
            speed = crouchSpeed;
        }
        else if (isGrounded)
        {
            // WALKING
            moveState = MoveState.Walk;
            speed = walkSpeed;
        }
        else
        { 
            // IN AIR?? FROM JUMPING???
            moveState = MoveState.InAir;
        }
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z;
        
        // track the first person position during FPS mode to make a heatmap
        var props = new Value();
        props["Player FPS Position on Map"] = transform.position;
        Analytics.LogAnalyticEvent("Player FPS Position on Map", props);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Crouch()
    {
        //Debug.Log("called crouch() function");
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //Debug.Log("entered crouch");
            transform.localScale = new Vector3(transform.localScale.x, crouchScale, transform.localScale.z);
            transform.position -= Vector3.down * startScale * crouchScale;
            //this.GetComponent<Rigidbody>().AddForce(Vector3.down * 3f, ForceMode.Impulse);
            
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, startScale, transform.localScale.z);
        }
        
    }

    void GravityUpdate()
    {
        // gravity
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravity * Time.deltaTime;
    }
}
