/* SETUP:
 * 1. Set tag of camera to "MainCamera"
 * 2. Make sure camera is not attached to player object
 * 3. Attach script to camera
 * 4. See FirstPersonMovementRB script to set up player object if needed
 * Last Updated: 9/9/20
 */

using UnityEngine;

public class FPSCameraRB : MonoBehaviour {

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


    float verticalRotation;

    Vector3 playerTop;

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
       // Application.targetFrameRate = 60;
    }

    void Start()
    {       
        cameraRotationInputVect = Vector3.zero;

        minAngle = -90.0f;
        maxAngle = 90.0f;
        rotationLerpSpeed = 25.0f;
        //positionLerpSpeed = 3.0f;
        cameraMouseSensitivity = 100.0f;
        cameraHeightOffset = 0.1f;

        playerTop = (player.transform.localPosition + Vector3.up * (playerCollider.height / 2));
    }

    void Update()
    {
        if (positionBehaviour == PositionStyle.Raw)
        {
            //CameraPositionRaw();
        }
        else if (positionBehaviour == PositionStyle.Lerp)
        {
         //   CameraPositionLerp();
        }
        CameraRotation();
    }

    void LateUpdate()
    {
        if (rotationBehaviour == RotationStyle.Raw)
        {
            //CameraRotationRaw();
        }
        else if (rotationBehaviour == RotationStyle.Lerp)
        {
        //    CameraRotationLerp();
        }
    }

    //*************************************************************************************************************************************************************************************
    
    void CameraRotation()
    {
        verticalRotation += Input.GetAxisRaw("Mouse Y") * cameraMouseSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, minAngle, maxAngle);


        if (rotationBehaviour == RotationStyle.Raw)
            transform.localEulerAngles = Vector3.left * verticalRotation;
        else
            transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(Vector3.left*verticalRotation), rotationLerpSpeed * Time.deltaTime);
    }
    
    
    void CameraRotationLerp()
    {
        cameraRotationInputVect.x -= Input.GetAxis("Mouse Y") * cameraMouseSensitivity * Time.deltaTime;
        //cameraRotationInputVect.y += Input.GetAxis("Mouse X") * cameraMouseSensitivity * Time.deltaTime;

        cameraRotationInputVect.x = Mathf.Clamp(cameraRotationInputVect.x, minAngle, maxAngle);
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * cameraMouseSensitivity * Time.deltaTime, 0, 0));
       // gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(cameraRotationInputVect.x, player.transform.rotation.eulerAngles.y, 0), rotationLerpSpeed * Time.deltaTime);

       // gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, Quaternion.Euler(cameraRotationInputVect.x, player.transform.rotation.eulerAngles.y, 0), rotationLerpSpeed * Time.deltaTime);
    }


    void CameraPositionLerp()
    {
        //   playerTop = (player.transform.localPosition + player.transform.up * (playerCollider.height / 2));

        //gameObject.transform.position = transform.parent.position + player.transform.up * (playerCollider.height / 2);
       // gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position,
        //    new Vector3(transform.position.x, playerTop.y - cameraHeightOffset, transform.position.z), ref velocity, positionLerpSpeed * Time.deltaTime);
    }

    void CameraPositionRaw()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, playerCollider.bounds.max.y - cameraHeightOffset, player.transform.position.z);
    }
}
