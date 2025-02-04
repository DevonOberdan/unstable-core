using UnityEngine;

public class ItemSway : MonoBehaviour
{
    [SerializeField] Rigidbody playerRB;

    [SerializeField] float mouseSwayForce = 1.5f;
    [SerializeField] float movementSwayForce = 1.5f;
    [SerializeField] float smoothVal = 10f;
    [SerializeField] float walkFactor = 10f;
    [SerializeField] float jumpRange = 10f;

    [SerializeField] bool tiltOnJump;

    [DrawIf(nameof(tiltOnJump), true)]
    [Range(0,10f)]
    [SerializeField] float jumpTiltVal = 10;

    Transform childTransform;
    Quaternion centerRotation, childCenterRotation;
    Quaternion newGoalRotation, newSwayRotation, newMovementRotation, newJumpRotation;

    float mouseX, mouseY;
    float inputMoveX, inputMoveY;

    void Start()
    {
        centerRotation = transform.localRotation;

        childTransform = transform.GetChild(0);
        childCenterRotation = childTransform.localRotation;
    }

    void Update()
    {
        HandleInput();
    }

    /* May want to try independent smoothing values for mouse sway vs. jumping sway, etc.
     * 
     * Dont think i can because logically can only Slerp to one new rotation at a time
     */

    private void FixedUpdate()
    {
        HandleMouseSway();
        HandleMovementSway();

        newGoalRotation = centerRotation * newSwayRotation * newMovementRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, newGoalRotation, smoothVal * Time.deltaTime);
        childTransform.localRotation = Quaternion.Slerp(childTransform.localRotation, childCenterRotation * newJumpRotation, jumpTiltVal * Time.deltaTime);
    }

    void HandleMouseSway()
    {
        Quaternion yRot = Quaternion.AngleAxis(-mouseX * mouseSwayForce, Vector3.up);
        Quaternion xRot = Quaternion.AngleAxis(mouseY * mouseSwayForce, Vector3.right);
        newSwayRotation = yRot * xRot;
    }

    void HandleMovementSway()
    {
        Vector3 localVelocity = playerRB.transform.InverseTransformDirection(playerRB.velocity);
        newJumpRotation = Quaternion.AngleAxis(Mathf.Clamp(localVelocity.y,-jumpRange, jumpRange), Vector3.right);

        Quaternion yRot = Quaternion.AngleAxis(-inputMoveX * movementSwayForce, Vector3.up);
        Quaternion xRot = Quaternion.AngleAxis(inputMoveY *Mathf.Sin(walkFactor * Time.realtimeSinceStartup) * .5f, Vector3.right);

        newMovementRotation = yRot * xRot * newJumpRotation;
    }

    void HandleInput()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        inputMoveX = Input.GetAxisRaw("Horizontal");
        inputMoveY = Input.GetAxisRaw("Vertical");
    }
}
