/* SETUP:
 * 1. Add default capsule object to scene
 * 2. Remove mesh renderer component
 * 3. Create physics material with zero-ed out values and friction combine set to multiply and drag reference into capsule collider component
 * 4. Set height of capsule collider component to 1.8288 (~6ft tall)
 * 5. Attach rigidbody component and set use gravity to off and freeze all rotations and set collision detection to continuous dynamic
 * 6. Set tag of capsule object to "Player"
 * 7. Attach this script to the capsule object
 * 8. See FirstPersonCameraRB script to set up camera if needed
 * 9. Set fixed update and maximum allowed timestep to desired rate (0.0083 and 0.01667 recommended)
 * Last Updated: 9/9/20
 */

using UnityEngine;

public class FirstPersonGravityMovementRB : MonoBehaviour
{
    GameObject playerCamera;
    Rigidbody playerRB;
    CapsuleCollider playerCapsuleCollider;

    Vector3 inputVect, inputDirection, movementDirection, groundNormal;

    Vector3 gravityDirection;
    GravityFirstPersonCameraRB cameraController;

    float currentMoveSpeed, defaultMoveSpeed, inAirLerpSpeed, groundedLerpSpeed;
    float crouchMoveSpeed, crouchStandingHeight, crouchCrouchingHeight;
    float jumpScalar, jumpBuffer;
    float gravityScalar, gravityThreshold, gravityEase;
    float currentHeight, previousHeight;

    int playerMask, jumpCounter;

    [SerializeField]
    GameObject world;

    void Awake()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        playerRB = GetComponent<Rigidbody>();
        playerCapsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        currentMoveSpeed = 6.0f;
        defaultMoveSpeed = currentMoveSpeed;
        inAirLerpSpeed = defaultMoveSpeed / 350.0f;
        groundedLerpSpeed = defaultMoveSpeed / 30.0f;
        crouchMoveSpeed = defaultMoveSpeed / 2.0f;
        crouchStandingHeight = 1.8288f;
        crouchCrouchingHeight = 1.2192f;
        jumpScalar = 8.0f;
        jumpBuffer = 0.25f;
        gravityScalar = 14.0f;
        gravityThreshold = gravityScalar / 1.5f;
        currentHeight = gameObject.transform.position.y;
        previousHeight = currentHeight;

        playerMask = ~(1 << LayerMask.NameToLayer("Player"));
        jumpCounter = 0;


        /***********/

        gravityDirection = Vector3.zero;
        cameraController = playerCamera.GetComponent<GravityFirstPersonCameraRB>();
    }

    void FixedUpdate()
    {
        PlayerMovement();
        PlayerGravity();

        //Vector3 dir = (world.transform.position - transform.position).normalized;
        Vector3 dir = (transform.position - world.transform.position).normalized;

        //print(gravityDirection);
        //Quaternion targetRotation = Quaternion.FromToRotation(transform.up, dir) * transform.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, dir) * transform.rotation;

        Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);


       // Quaternion camRotation = Quaternion.FromToRotation(newRotation, playerCamera.transform.eulerAngles.y, transform.rotation.z);



        transform.rotation = newRotation;


        //transform.localEulerAngles = new Vector3(transform.rotation.x, playerCamera.transform.eulerAngles.y, transform.rotation.z);
       // transform.Rotate(transform.up * Input.GetAxis("Mouse X") * cameraController.cameraMouseSensitivity * Time.deltaTime * 3.5f);



        print(groundNormal);

       //transform.localRotation = Quaternion.Euler(targetRotation.eulerAngles.x, transform.localRotation.y, targetRotation.eulerAngles.z);
    }

    void Update()
    {
        PlayerMovementHelper();
        PlayerJump();
        PlayerCrouch();
    }

    //***************************************************************************************************************************************************************************************************
    private void PlayerMovement()
    {        
        if (IsMoving())
        {
            if (IsGrounded())
            {
                playerRB.velocity = Vector3.Lerp(playerRB.velocity, movementDirection - groundNormal * 2.0f, groundedLerpSpeed);
            }
            else if (!IsGrounded())
            {
                playerRB.velocity = Vector3.Lerp(playerRB.velocity, new Vector3(inputDirection.x * defaultMoveSpeed, playerRB.velocity.y, inputDirection.z * defaultMoveSpeed), inAirLerpSpeed);
            }
        }
        else if (!IsMoving())
        {
            if (IsGrounded())
            {
                playerRB.velocity = Vector3.zero;
            }
        }
    }

    private void PlayerMovementHelper()
    {
        currentHeight = gameObject.transform.position.y;

       // gameObject.transform.localRotation = Quaternion.Euler(transform.rotation.x, playerCamera.transform.eulerAngles.y, transform.rotation.z);

        inputVect = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        inputDirection = gameObject.transform.TransformDirection(inputVect);

        float yDirection = (-groundNormal.x * inputDirection.x - groundNormal.z * inputDirection.z) / groundNormal.y;
        movementDirection = new Vector3(inputDirection.x, yDirection, inputDirection.z).normalized * currentMoveSpeed;
       
        if (Physics.Raycast(gameObject.transform.localPosition + inputDirection.normalized * 0.25f, -transform.up, out RaycastHit hit, Mathf.Infinity, playerMask, QueryTriggerInteraction.Ignore) 
            && currentHeight != previousHeight)
        {
            Debug.DrawRay(hit.point, hit.normal * hit.distance, Color.yellow);

            groundNormal = hit.normal;
            previousHeight = currentHeight;
        }
        else if (Physics.Raycast(gameObject.transform.localPosition, -transform.up, out hit, Mathf.Infinity, playerMask, QueryTriggerInteraction.Ignore) && currentHeight == previousHeight)
        {
            //  Debug.DrawRay(transform.position, -transform.up * hit.distance, Color.yellow);
            print("currHeight is same as previous");

            groundNormal = hit.normal;
        }
    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() || Input.GetKeyDown(KeyCode.Space) && jumpBuffer >= 0.0f)
        {
            if (jumpCounter == 0)
            {
                playerRB.velocity = new Vector3(playerRB.velocity.x, 0, playerRB.velocity.z);
                playerRB.AddForce(transform.up * jumpScalar, ForceMode.VelocityChange);
            }
            jumpCounter = 1;
        }

        if (!IsGrounded())
        {
            jumpBuffer -= Time.deltaTime;
        }
        else if (IsGrounded() && jumpBuffer <= 0.0f)
        {
            jumpBuffer = 0.25f;
            jumpCounter = 0;
        }
    }

    private void PlayerCrouch()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed = crouchMoveSpeed;

            if (IsGrounded() && !IsCrouching())
            {
                playerCapsuleCollider.height = crouchCrouchingHeight;
                playerCapsuleCollider.center = new Vector3(0, -0.4144f + 0.1096f, 0);
            }
            else if (!IsGrounded())
            {
                playerCapsuleCollider.height = crouchCrouchingHeight;
                playerCapsuleCollider.center = new Vector3(0, 0.4144f - 0.1096f, 0);
            }
        }
        else if (!Physics.SphereCast(transform.TransformPoint(playerCapsuleCollider.center), 0.49f, transform.up, out _, 0.8288f, playerMask, QueryTriggerInteraction.Ignore))
        {
            playerCapsuleCollider.height = crouchStandingHeight;
            playerCapsuleCollider.center = Vector3.zero;
            currentMoveSpeed = defaultMoveSpeed;
        }
    }

    private void PlayerGravity()
    {

      //  CalculateNewGravity();

        if (!IsGrounded())
        {
            playerRB.AddForce(-transform.up * gravityScalar, ForceMode.Acceleration);

            if (playerRB.velocity.y < -gravityThreshold)
            {
                gravityEase = Mathf.Abs(playerRB.velocity.y / gravityScalar);
                playerRB.AddForce(-transform.up * gravityEase * gravityScalar);
            }
            else if (playerRB.velocity.y > gravityThreshold)
            {
                gravityEase = Mathf.Abs(playerRB.velocity.y / gravityScalar);
                playerRB.AddForce(-transform.up * gravityEase * gravityScalar);
            }
        }
    }

    void CalculateNewGravity()
    {
        RaycastHit hit; 

        if(Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity))
        {

            gravityDirection = -hit.normal;
        }
    }

    //***************************************************************************************************************************************************************************************************
    public bool IsGrounded()
    {
        if (Physics.SphereCast(new Vector3(gameObject.transform.position.x, playerCapsuleCollider.bounds.min.y + 0.5f, gameObject.transform.position.z), 0.49f, -groundNormal, out _, 0.05f, playerMask, QueryTriggerInteraction.Ignore))
            //&& Physics.Raycast(new Vector3(gameObject.transform.position.x, playerCapsuleCollider.bounds.min.y + 0.5f, gameObject.transform.position.z), Vector3.down, 0.72f, playerMask, //QueryTriggerInteraction.Ignore))
        {
            return true;
        }
        else return false;
    }

    public bool IsMoving()
    {
        if (inputVect.magnitude != 0)
        {
            return true;
        }
        else return false;
    }

    public bool IsCrouching()
    {
        if (playerCapsuleCollider.height == crouchCrouchingHeight)
        {
            return true;
        }
        else return false;
    }
}
