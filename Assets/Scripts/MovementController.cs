using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MovementController : MonoBehaviour
{
    //Variables
    PlayerController controls;

    public float accel = 6.0F;
    public float accelSmoothTime = 0.5f;
    public float maxSpeed = 14.0f;
    public float airMoveMod = 0.85f;
    public float minSpeed = 0.2f;

    public float jumpSpeed = 8.0F;
    public float jumpSmoothTime = 1.0f;
    public float doubleJumpMod = 1.4f;

    public float turnSmoothTime = 0.1f;
    public float decelerationMod = .6f;
    public float deccelerationSmoothTime = 4;

    public float maxGroundAngle = 120;
    public float groundDistPad = -0.2f;


    public AnimationCurve accelerationCurve;
    public AnimationCurve decelerationCurve;
    public Transform cam;



    private PlayerAnimationController animCon;

    private bool onGround = true;
    private float distToGround;
    private float accelerationStartTime = 0.0f;
    private float deccelerationStartTime = 0.0f;
    private float jumpStartTime = 0.0f;
    private float jumpAcceleration = 0.0f;
    private float airbourne = 1.0f;
    private float groundAngle;  
    private float sphereCastWidth;


    private float angle = 0.0f;

    private RaycastHit hitInfo;

    private Vector3 inputDirection = Vector3.zero;
    private Vector3 forwardDir;
    private float turnSmoothVelocity;

    private Rigidbody rb;

    private void Start()
    {
    }

    private void Awake()
    {
        //intializes variables
        controls = new PlayerController();
        rb = GetComponent<Rigidbody>();
        animCon = GetComponent<PlayerAnimationController>();

        //gets dist to bottom of collider
        distToGround = GetComponent<Collider>().bounds.extents.y;
        sphereCastWidth = GetComponent<Collider>().bounds.extents.x / 2.2f;

        //sets up input listening events
        controls.Movement.Jump.performed += ctx => { if (ctx.interaction is TapInteraction) { getInputJump(1.0f); } };
        controls.Movement.Jump.canceled += ctx => { if (ctx.interaction is HoldInteraction) { getInputJump(doubleJumpMod); } };


        controls.Movement.LSMove.performed += ctx => getInputDirection( ctx.ReadValue<Vector2>());
        controls.Movement.LSMove.canceled += ctx => getInputDirection(Vector2.zero);

        

    }

    void Update()
    {
        passAnimationData();
    }

    void passAnimationData()
    {
        Vector3 perp = Vector3.Cross(rb.velocity.normalized, forwardDir.normalized);
        if(Vector3.Dot(perp, Vector3.up) != 0 &&  Vector3.Dot(rb.velocity.normalized, forwardDir.normalized) != 0)
            animCon.setDirectionAnim(Vector3.Dot(perp, Vector3.up),  Vector3.Dot(rb.velocity.normalized, forwardDir.normalized));
        animCon.setVelocityAnim(rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        //boolean to check if player is grounded or not
        onGround = isGrounded();
        calcAirbourne();

        calcForward();
        calcGroundAngle();

        toggleGravity();
            
        //handles jumping
        jump();



               
        
        //accelerates when below max spee
        if (inputDirection.magnitude >= .01) {

            rotate();
            move();

        }
        
        
        


    }

    void rotate()
    {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            rb.MoveRotation(Quaternion.Euler(0f, angle, 0f));
    }

    void move()
    {

        float sm = accel * accelerationCurve.Evaluate((Time.time - accelerationStartTime) / accelSmoothTime ); // Mathf.SmoothStep(0.5f, accel, (Time.time - accelerationStartTime) / accelSmoothTime);
                                                                                                               //Debug.Log(sm + " - " + accelerationCurve.Evaluate((Time.time - accelerationStartTime) / accelSmoothTime));
        
        
        if (rb.velocity.magnitude < maxSpeed && inputDirection.magnitude >= .15 && groundAngle <= maxGroundAngle)
        {

            rb.AddForce(sm * airbourne * (forwardDir.normalized * Time.deltaTime), ForceMode.Impulse);
        }
    }

    void decelerate()
    {
        if (onGround)
        {
            float deceleration = decelerationCurve.Evaluate((Time.time - deccelerationStartTime) / deccelerationSmoothTime);
            rb.velocity = rb.velocity - (rb.velocity * decelerationMod);
            if (rb.velocity.sqrMagnitude < minSpeed)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    void jump()
    {
        if (jumpAcceleration > 0)
        {
            jumpAcceleration = jumpAcceleration * Mathf.SmoothStep(1, 0, (Time.time - jumpStartTime) / jumpSmoothTime);
        }

        rb.AddRelativeForce((new Vector3(0, jumpAcceleration, 0) * Time.deltaTime), ForceMode.Impulse);
        jumpAcceleration *= 0.7f;

    }

    

    //inputs movement direction
    void getInputDirection(Vector2 vec)
    {
        if(Vector2.zero == vec)
        {
            accelerationStartTime = 0;
            deccelerationStartTime = Time.time;
        }
        else if(accelerationStartTime == 0)
            accelerationStartTime = Time.time;
        inputDirection.x = vec.x;
        inputDirection.z = vec.y;
    }

    //Jump function called by input, sets jump amplitutde
    void getInputJump(float j)
    {
        if (onGround == true) { 
            jumpAcceleration = jumpSpeed * j;
            jumpStartTime = Time.time;
        }
    }

    void calcAirbourne()
    {
        airbourne = 1.0f;
        if (!onGround)
        {
            airbourne = airMoveMod;
        }
    }

    void toggleGravity()
    {
        if (!onGround || groundAngle >= maxGroundAngle)
        {
            rb.useGravity = true;
        }
        else
        {
            rb.useGravity = false;
        }
    }

    //returns whether the player is on the ground or not.
    private bool isGrounded()
    {
        int layerMask = LayerMask.GetMask("Ground");
        return Physics.SphereCast(rb.transform.position, sphereCastWidth, Vector3.down, out hitInfo, distToGround + groundDistPad, layerMask);
        
                   
    }

    private void calcForward (){
        if (!onGround)
        {
            forwardDir = rb.transform.forward;
            return;
        }

        forwardDir = Vector3.Cross(hitInfo.normal, -rb.transform.right);

        Debug.DrawLine(rb.transform.position, rb.transform.position + forwardDir * (distToGround + groundDistPad), Color.red);
    }

    private void calcGroundAngle()
    {
        if (!onGround)
        {
            groundAngle = 90;
            return;
        }

        groundAngle = Vector3.Angle(hitInfo.normal, rb.transform.forward);
    }


    //Ensure the inputmap controls gets enabled and disabled with the rest of the object
    void OnEnable()
    {
        controls.Movement.Enable();
    }

    private void OnDisable()
    {
        controls.Movement.Disable();
    }



}
