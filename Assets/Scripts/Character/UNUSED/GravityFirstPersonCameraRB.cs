/* SETUP:
 * 1. Set tag of camera to "MainCamera"
 * 2. Make sure camera is not attached to player object
 * 3. Attach script to camera
 * 4. See FirstPersonMovementRB script to set up player object if needed
 * Last Updated: 9/9/20
 */

using UnityEngine;

public class GravityFirstPersonCameraRB : MonoBehaviour {

    GameObject player;
    CapsuleCollider playerCollider;

    public Vector3 cameraRotationInputVect;
    Vector3 velocity;

    float minAngle;
    float maxAngle;
    public float rotationLerpSpeed;
    float positionLerpSpeed;
    public float cameraMouseSensitivity;
    float cameraHeightOffset;

    public enum PositionStyle
    {
        Raw, Lerp
    }
    public PositionStyle positionBehaviour = new PositionStyle();

    public enum RotationStyle
    {
        Raw, Lerp
    }
    public RotationStyle rotationBehaviour = new RotationStyle();


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<CapsuleCollider>();

       // QualitySettings.vSyncCount = 1;
      //  Application.targetFrameRate = 60;
    }

    void Start()
    {       
        cameraRotationInputVect = gameObject.transform.rotation.eulerAngles;

        minAngle = -90.0f;
        maxAngle = 90.0f;
        rotationLerpSpeed = 25.0f;
        positionLerpSpeed = 3.0f;
        cameraMouseSensitivity = 100.0f;
        cameraHeightOffset = 0.1f;
    }

    void Update()
    {
        if (positionBehaviour == PositionStyle.Raw)
        {
            CameraPositionRaw();
        }
        else if (positionBehaviour == PositionStyle.Lerp)
        {
            CameraPositionLerp();
        }
    }

    void LateUpdate()
    {
        if (rotationBehaviour == RotationStyle.Raw)
        {
            CameraRotationRaw();
        }
        else if (rotationBehaviour == RotationStyle.Lerp)
        {
            CameraRotationLerp();
        }
    }

    //*************************************************************************************************************************************************************************************
    void CameraRotationLerp()
    {
        cameraRotationInputVect.x -= Input.GetAxis("Mouse Y") * cameraMouseSensitivity * Time.deltaTime;
        cameraRotationInputVect.y += Input.GetAxis("Mouse X") * cameraMouseSensitivity * Time.deltaTime;

        cameraRotationInputVect.x = Mathf.Clamp(cameraRotationInputVect.x, minAngle, maxAngle);

       // gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(cameraRotationInputVect.x, 0, 0), rotationLerpSpeed * Time.deltaTime);

        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(cameraRotationInputVect.x, cameraRotationInputVect.y, 0), rotationLerpSpeed * Time.deltaTime);
    }

    void CameraRotationRaw()
    {
        cameraRotationInputVect.x -= Input.GetAxis("Mouse Y") * cameraMouseSensitivity * Time.deltaTime;
        cameraRotationInputVect.y += Input.GetAxis("Mouse X") * cameraMouseSensitivity * Time.deltaTime;

        cameraRotationInputVect.x = Mathf.Clamp(cameraRotationInputVect.x, minAngle, maxAngle);

        gameObject.transform.rotation = Quaternion.Euler(cameraRotationInputVect.x, cameraRotationInputVect.y, 0);
    }

    void CameraPositionLerp()
    {
        gameObject.transform.localPosition = Vector3.SmoothDamp(gameObject.transform.position,
            new Vector3(player.transform.localPosition.x, playerCollider.bounds.max.y - cameraHeightOffset, player.transform.localPosition.z), ref velocity, positionLerpSpeed * Time.deltaTime);
    }

    void CameraPositionRaw()
    {
        gameObject.transform.localPosition = new Vector3(player.transform.localPosition.x, playerCollider.bounds.max.y - cameraHeightOffset, player.transform.localPosition.z);
    }
}
