using UnityEngine;

public class FirstPersonSphereCameraRB : MonoBehaviour
{
    GameObject player;
    FirstPersonSphereMovementRB playerMovementScript;
    Quaternion playerRotation, cameraRotation;
    Vector3 playerPosition, cameraPosition, cameraInputRotation, refVelocity;

    private float cameraHeight, cameraMinAngle, cameraMaxAngle, rotationLerpSpeed, positionLerSpeed;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovementScript = player.GetComponent<FirstPersonSphereMovementRB>();
    }

    void Start()
    {
        playerPosition = player.transform.position;
        playerRotation = player.transform.rotation;

        gameObject.transform.position = playerPosition + (player.transform.up * cameraHeight);
        gameObject.transform.rotation = playerRotation;
        cameraInputRotation = playerRotation.eulerAngles;

        cameraHeight = 0.8f; //distance camera is from the ground, ideally placed at eye level
        cameraMinAngle = 89.0f; //mininmum angle the camera can look down
        cameraMaxAngle = 89.0f; //maximum angle the camera can look up
        rotationLerpSpeed = 0.4f; //speed at which camera rotates toward destination
        positionLerSpeed = 0.05f; //speed at which camera follows player position
    }

    //update player and input data asap and continuously
    void Update()
    {
        playerPosition = player.transform.position;
        playerRotation = player.transform.rotation;

        cameraInputRotation.x += -Input.GetAxis("Mouse Y") * playerMovementScript.yRotateSpeed;
        cameraInputRotation.x = Mathf.Clamp(cameraInputRotation.x, -cameraMinAngle, cameraMaxAngle);

        cameraRotation = Quaternion.Euler(cameraInputRotation);
        cameraPosition = playerPosition + (player.transform.up * cameraHeight);
    }

    //update camera position and rotation after player and input data has already been updated
    private void LateUpdate()
    {
        gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, cameraPosition, ref refVelocity, positionLerSpeed);

        Quaternion combinedRotation = playerRotation * cameraRotation;
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, combinedRotation, rotationLerpSpeed);
    }
}
