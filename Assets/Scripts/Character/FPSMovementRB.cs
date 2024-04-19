﻿/* SETUP:
 * 1. Add default capsule object to scene
 * 2. Remove mesh renderer component
 * 3. Create physics material with zero-ed out values and friction combine set to multiply and drag reference into capsule collider component
 * 4. Set height of capsule collider component to 1.8288 (~6ft tall)
 * 5. Attach rigidbody component and set use gravity to off and freeze all rotations and set collision detection to continuous dynamic
 * 6. Set tag of capsule object to "Player"
 * 7. Attach this script to the capsule object
 * 8. See FirstPersonCameraRB script to set up camera if needed
 * 9. Set fixed update and maximum allowed timestep to desired rate (0.0083 and 0.01667 recommended)
 * 
 * Last Updated: 4/4/22
 * 
 *  Issues remaining are:
 *   
 * 1. Weird behavior with crouching, but we likely just won't use it.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class FPSMovementRB : FPSMovement
{
    [SerializeField] float jumpFactor = 8f;
    [SerializeField] float defaultMoveSpeed = 6f;
    [SerializeField] float sprintSpeedFactor = 1.75f;

    [Header("Dampening Values")]
    [Tooltip("Time it takes to lerp in-air speed from ground speed down to the divided air speed. Higher values retains ground speed longer.")]
    [SerializeField] float timeToLerpAirSpeedFromMoving = 4f;

    [Tooltip("Time it takes to lerp in-air speed from defaultMoveSpeed down to the divided air speed. Value < 1 to limit air-steering due to lack of momentum.")]
    [SerializeField] float timeToLerpAirSpeedFromStill = .2f;

    [SerializeField] float airSpeedQuotient;

    float oldAirSpeedQuotient;

    Rigidbody playerRB;
    GravityObject playerGravity;

    Vector3 inputVect, targetMoveAmount, movementAmount;
    Vector3 smoothMoveVel;

    float currentMoveSpeed, sprintSpeed, activeGroundSpeed, moveLerpSpeed;

    float jumpBuffer, jumpCooldown, timeSinceJump;
    float timeToLerpAirSpeed, speedAtJump, inAirSmoothLerpTime;
    
    float standingTime;

    int playerMask, jumpCounter;
    bool inAir;

    readonly int MAX_JUMPS = 1;

    public float AirSpeedQuotient { get => airSpeedQuotient; set => airSpeedQuotient = value; }
    public float OldAirSpeedQuotient { get => oldAirSpeedQuotient; set => oldAirSpeedQuotient = value; }

    public GravityObject PlayerGravity => playerGravity;

    public float JumpFactor => jumpFactor;

    public override bool IsGrounded => Physics.SphereCast(gameObject.transform.position, 0.45f, -gameObject.transform.up, out _, 0.47f, playerMask, QueryTriggerInteraction.Ignore); // basically the same
    public override bool IsInAir => !IsGrounded; // the same
    public override bool IsIdle => movementAmount.magnitude < 3f && inputVect == Vector3.zero;
    public override bool IsFalling => !IsGrounded && Vector3.Dot(playerRB.velocity.normalized, playerGravity.GravityDirection) > 0; // the same
    public override bool IsSprinting => currentMoveSpeed == sprintSpeed && inputVect.z > 0;
    public override bool IsRunning => IsGrounded && !IsSprinting && !IsIdle;  //the same

    bool stallInput;
    public bool StallInput
    {
        get => stallInput;
        set
        {
            stallInput = value;
            if (stallInput)
            {
                //targetMoveAmount = inputVect * currentMoveSpeed;

                movementAmount = Vector3.zero;//Vector3.Lerp(movementAmount, Vector3.zero, 0.9f);
            }
        }
    }

    void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
        playerGravity = GetComponent<GravityObject>();
    }

    void Start()
    {
        currentMoveSpeed = defaultMoveSpeed;
        sprintSpeed = defaultMoveSpeed * sprintSpeedFactor;
        activeGroundSpeed = defaultMoveSpeed;

        moveLerpSpeed = defaultMoveSpeed / 30.0f; // 0.2

        jumpBuffer = 0.25f;
        jumpCooldown = 0.2f;

        playerMask = ~(1 << LayerMask.NameToLayer("Player"));
        jumpCounter = 0;
    }

    void FixedUpdate()
    {
        PlayerMovement();
        gameObject.transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
    }

    void Update()
    {
        if (GameManager.Instance.AcceptingGameInput == false)
            return;

        PlayerMovementHelper();
        PlayerJump();
        Juice.Instance.ExpandFOV(IsSprinting);
    }

    private void PlayerMovement()
    {
        playerRB.MovePosition(playerRB.position + transform.TransformDirection(movementAmount)* Time.fixedDeltaTime);

        if (IsGrounded)
        {
            standingTime += Time.fixedDeltaTime;
            standingTime = Mathf.Clamp(standingTime, 0, 1);
            playerRB.velocity = Vector3.Lerp(playerRB.velocity, Vector3.zero, standingTime);
        }
        else
        {
            standingTime = 0f;
        }
    }

    private void PlayerMovementHelper()
    {

        HandleInput();

        if (!stallInput)
        {
            targetMoveAmount = inputVect * currentMoveSpeed;
            movementAmount = Vector3.SmoothDamp(movementAmount, targetMoveAmount, ref smoothMoveVel, moveLerpSpeed);
        }


        if (IsGrounded)
        {
            if (inAir)
            {
                inAir = false;
                currentMoveSpeed = activeGroundSpeed;
                speedAtJump = 0;
            }
        }
        else
        {
            if (!inAir)
                JumpStart();

            CalculateAirSpeed();
        }
    }

    void HandleInput()
    {
        inputVect = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SetMoveSpeed(sprintSpeed);
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            SetMoveSpeed(defaultMoveSpeed);
    }

    void SetMoveSpeed(float speed)
    {
        if (IsGrounded)
            currentMoveSpeed = speed;
        
        activeGroundSpeed = speed;
    }

    void JumpStart()
    {
        if (inputVect.magnitude > 0)
            timeToLerpAirSpeed = timeToLerpAirSpeedFromMoving;
        else
            timeToLerpAirSpeed = timeToLerpAirSpeedFromStill;

        speedAtJump = activeGroundSpeed;

        inAirSmoothLerpTime = 0;
        inAir = true;
    }

    void CalculateAirSpeed()
    {
        inAirSmoothLerpTime += Time.deltaTime;
        Mathf.Clamp(inAirSmoothLerpTime, 0, timeToLerpAirSpeed);

        currentMoveSpeed = Mathf.Lerp(speedAtJump, speedAtJump / airSpeedQuotient, inAirSmoothLerpTime / timeToLerpAirSpeed);
    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (IsGrounded || jumpBuffer >= 0.0f) && jumpCounter<MAX_JUMPS)
        {
            ForceJump(jumpFactor);
            jumpCounter++;
            timeSinceJump = 0f;
        }

        if (!IsGrounded)
        {
            jumpBuffer -= Time.deltaTime;
        }
        else if (IsGrounded && jumpCounter>0 && timeSinceJump > jumpCooldown)
        {
            jumpBuffer = 0.25f;
            jumpCounter = 0;
        }

        if (timeSinceJump < jumpCooldown)
            timeSinceJump += Time.deltaTime;
    }

    public void ForceJump(float factor, ForceMode force = ForceMode.VelocityChange) => playerRB.AddForce(transform.up * factor, force);
    public void ForceForwardJump(float factor, ForceMode force = ForceMode.VelocityChange) => playerRB.AddForce(transform.TransformDirection(movementAmount).normalized * factor, force);
}