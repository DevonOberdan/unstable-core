/* SETUP:
 * 1. Set tag of camera to "MainCamera"
 * 2. Make sure camera is not attached to player object
 * 3. Attach script to camera
 * 4. See FirstPersonMovementRB script to set up player object if needed
 * Last Updated: 9/9/20
 */

using UnityEngine;

public class FirstPersonCameraRB : MonoBehaviour {

    GameObject player;
    CapsuleCollider playerCollider;

    Vector3 cameraRotationInputVect;
    Vector3 velocity;

    float minAngle;
    float maxAngle;
    float rotationLerpSpeed;
    float positionLerpSpeed;
    float cameraMouseSensitivity;
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

        //QualitySettings.vSyncCount = 1;
        //Application.targetFrameRate = 60;
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

        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(cameraRotationInputVect.x, cameraRotationInputVect.y, 0), rotationLerpSpeed * Time.deltaTime);
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
        gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position,
            new Vector3(player.transform.position.x, playerCollider.bounds.max.y - cameraHeightOffset, player.transform.position.z), ref velocity, positionLerpSpeed * Time.deltaTime);
    }

    void CameraPositionRaw()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, playerCollider.bounds.max.y - cameraHeightOffset, player.transform.position.z);
    }
}
