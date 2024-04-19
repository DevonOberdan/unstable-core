/* SETUP:
 * 1. Add default capsule object to scene
 * 2. Remove mesh renderer component
 * 3. Create physics material with zero-ed out values and friction combine set to multiply and drag reference into capsule collider component
 * 4. Set height of capsule collider component to 1.8288 (~6ft tall)
 * 5. Attach rigidbody component and set use gravity to off and freeze all rotations and set collision detection to continuous dynamic
 * 6. Set tag of capsule object to "Player"
 * 7. Attach this script to the capsule object
 * 8. See FirstPersonSphereCameraRB script to set up camera if needed
 * 9. Set fixed update and maximum allowed timestep to desired rate (0.0083 and 0.01667 recommended)
 * Last Updated: 3/12/23
 */

using UnityEngine;

public class FirstPersonSphereMovementRB : FPSMovement
{
    private Rigidbody playerRB;
    private Vector3 gravityDirection, rotationDirection, toCenterGravityDirection, awayFromCenterGravityDirection, forwardDirection, rightDirection, moveDirection, moveVelocity;

    private float runScalar, sprintScalar, jumpScalar, gravityScalar, rampVelocityScalar, inAirVelocityScalar, maxVelocityScalar;
    [HideInInspector] public float yRotateSpeed;

    private int playerMask;

    private bool isGravityTowardsCenter, isGravityFlipping;

    enum FlipDirection { FORWARD, BACKWARD, LEFT, RIGHT };
    [SerializeField] FlipDirection flipDirection = FlipDirection.FORWARD;

    [SerializeField] float gravityFlipScalar = 4.0f;
    [SerializeField] float gravityFlipBoostFactor;

    public bool EnteringPlanet => isGravityTowardsCenter && currentDistance < FlipDistance;
    public bool ExitingPlanet => !isGravityTowardsCenter && currentDistance > FlipDistance;

    public float FlipDistance { get; private set; }
    float currentDistance;

    //standard movement states
    public override bool IsGrounded => Physics.SphereCast(gameObject.transform.position, 0.45f, -transform.up, out _, 0.4744f, playerMask, QueryTriggerInteraction.Ignore);
    public override bool IsInAir => !IsGrounded;
    public override bool IsFalling => !IsGrounded && Vector3.Dot(playerRB.velocity.normalized, gravityDirection) > 0;
    public override bool IsIdle => IsGrounded && moveDirection.magnitude == 0;
    public override bool IsSprinting => IsGrounded && Input.GetKey(KeyCode.LeftShift);
    public override bool IsRunning => IsGrounded && !IsSprinting && !IsIdle;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();

        Vector3 planetExtents = GameObject.FindGameObjectWithTag("Planet").GetComponent<MeshRenderer>().bounds.extents;
        float avgRadius = (planetExtents.x + planetExtents.y + planetExtents.z) / 3;
        FlipDistance = avgRadius;
    }

    private void Start()
    {
        runScalar = 5.0f; //default speed that player moves
        sprintScalar = 1.5f; //sprint speed logic is run speed times a percentage (ex: 5.0 x 1.5 = 7.5)
        jumpScalar = 5.0f; //determines how high player jumps
        gravityScalar = 9.8f; //gravity mimicks real world gravity
        rampVelocityScalar = 10.0f; //determines how quickly player gets to max velocity and how quickly they stop
        inAirVelocityScalar = 0.25f; // in air speed logic is velocity vector times a percentage (ex: 5.0 x 0.25 = 1.25)
        maxVelocityScalar = 15.0f; //max speed player can reach regardless of how they are moving
        //gravityFlipScalar = 4.0f; // determines how quickly the gravity vector rotates when gravity is flipping
        yRotateSpeed = 3.0f; //rotate speed for y direction that is shared with camera script to match rotate speed for x direction

        playerMask = ~(1 << LayerMask.NameToLayer("Player")); //mask used for ignoring player collider when using raycast for grounded check

        isGravityTowardsCenter = true;
        isGravityFlipping = false;
    }

    //functions that use input should be in update
    void Update()
    {
        currentDistance = Vector3.Distance(transform.position, Vector3.zero);
        HandleDistance();

        PlayerMovementHelper();
        PlayerGravityHelper();
        PlayerJump();
        PlayerSprint();
    }

    //functions that handle physics over time should be in fixed update
    void FixedUpdate()
    {
        PlayerMovement();
        PlayerGravity();
    }

    private void PlayerMovementHelper()
    {
        //player y rotation
        Quaternion yRotation = Quaternion.Euler(0f, Input.GetAxis("Mouse X") * yRotateSpeed, 0f);
        transform.rotation *= yRotation;

        //player xz rotation
        Quaternion xzRotation;
        xzRotation = Quaternion.FromToRotation(transform.up, -(gravityDirection).normalized);
        xzRotation *= transform.rotation;
        transform.rotation = xzRotation;

        //calc direction vectors for each movement direction
        forwardDirection = Vector3.Cross(transform.up, -transform.right) * Input.GetAxisRaw("Vertical");
        rightDirection = Vector3.Cross(transform.up, transform.forward) * Input.GetAxisRaw("Horizontal");

        //calc move direction vector based on direction vectors
        moveDirection = (forwardDirection + rightDirection).normalized * runScalar;
    }

    private void PlayerMovement()
    {
        //apply force to player based on difference between current rigidbody velocity and move direction
        //velocity vect y is zero'd out so that force is only applied in xz direction
        moveVelocity = transform.InverseTransformDirection(moveDirection - playerRB.velocity) * rampVelocityScalar;
        moveVelocity.y = 0;
        moveVelocity = transform.TransformDirection(moveVelocity);

        if (IsInAir) moveVelocity *= inAirVelocityScalar;

        playerRB.AddForce(moveVelocity, ForceMode.Acceleration);

        //clamp velocity if it reaches a maximum speed
        playerRB.velocity = Vector3.ClampMagnitude(playerRB.velocity, maxVelocityScalar);
    }

    private void PlayerJump()
    {
        //if space key is pressed while player is grounded, then apply an impulse force upwards based on player transform up direction
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded) playerRB.AddForce(transform.up * jumpScalar, ForceMode.VelocityChange);
    }

    private void PlayerSprint()
    {
        //if player is sprinting multiply move direction by factor of base speed
        if (IsSprinting) moveDirection *= sprintScalar;
    }

    private void PlayerGravityHelper()
    {
        //calculate gravity rotation directions continuously
        toCenterGravityDirection = Vector3.zero - gameObject.transform.position;
        awayFromCenterGravityDirection = -toCenterGravityDirection;

        //when gravity is not flipping, set gravity direction based on which side of sphere player is on
        //otherwise if gravity is flipping, calculate rotation direction and rotate gravity direction vector towards the appropriate direction
        //once gravity direction has reached its goal, gravity is no longer flipping and normal calculations resume
        if (!isGravityFlipping)
        {
            if (isGravityTowardsCenter) gravityDirection = toCenterGravityDirection;
            else gravityDirection = awayFromCenterGravityDirection;
        }
        else
        {
            rotationDirection = Vector3.Cross(RotationDirection, gravityDirection).normalized;

            if (isGravityTowardsCenter)
            {
                if (Vector3.Angle(gravityDirection, toCenterGravityDirection) >= 3.0f) 
                    gravityDirection = Vector3.RotateTowards(gravityDirection, toCenterGravityDirection + rotationDirection, gravityFlipScalar * Time.deltaTime, 0.0f);
                else 
                    isGravityFlipping = false;
            }
            else if (!isGravityTowardsCenter)
            {
                if (Vector3.Angle(gravityDirection, awayFromCenterGravityDirection) >= 3.0f) 
                    gravityDirection = Vector3.RotateTowards(gravityDirection, awayFromCenterGravityDirection + rotationDirection, gravityFlipScalar * Time.deltaTime, 0.0f);
                else 
                    isGravityFlipping = false;
            }
        }
    }

    void HandleDistance()
    {
        if (IsInAir && EnteringPlanet)
            SetGravity();
        else if (IsInAir && ExitingPlanet)
            SetGravity();

        void SetGravity()
        {
            isGravityTowardsCenter = !isGravityTowardsCenter;
            isGravityFlipping = true;
        }
    }

    //on trigger events used to start gravity flip transition 
    //private void OnTriggerEnter(Collider other)
    //{
    //    isGravityTowardsCenter = !isGravityTowardsCenter;
    //    isGravityFlipping = true;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    isGravityTowardsCenter = !isGravityTowardsCenter;
    //    isGravityFlipping = true;
    //}

    private void PlayerGravity()
    {
        //contantly add force in direction of gravity direction vector

        float finalScalar = gravityScalar;
        if (isGravityFlipping)
            finalScalar *= gravityFlipBoostFactor;

        playerRB.AddForce(gravityDirection.normalized * finalScalar, ForceMode.Acceleration);
    }

    private void DrawMovementVectors()
    {
        //draw movement related vectors
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + forwardDirection, Color.blue);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + rightDirection, Color.blue);
        Debug.DrawLine(gameObject.transform.position + gameObject.transform.up * 0.25f, (gameObject.transform.position + gameObject.transform.up * 0.25f) + moveDirection, Color.blue);
        Debug.DrawLine(gameObject.transform.position - gameObject.transform.up * 0.25f, (gameObject.transform.position - gameObject.transform.up * 0.25f) + moveVelocity, Color.red);

        //draw gravity related vectors
        Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position - gameObject.transform.up) + gravityDirection, Color.cyan);
        Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position - gameObject.transform.up) + toCenterGravityDirection, Color.green);
        Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position - gameObject.transform.up) + awayFromCenterGravityDirection, Color.green);
    }

    Vector3 RotationDirection 
    {
        get {
            return flipDirection switch
            {
                FlipDirection.FORWARD   => transform.right,
                FlipDirection.BACKWARD  => -transform.right,
                FlipDirection.LEFT      => transform.forward,
                FlipDirection.RIGHT     => -transform.forward,
                _ => transform.forward,
            };
        }
    }
}
