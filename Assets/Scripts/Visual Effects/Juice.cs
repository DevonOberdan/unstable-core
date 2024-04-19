using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juice : MonoBehaviour
{

    public static Juice Instance;




    [SerializeField]
    float fovTimeScale = 10f;

    [SerializeField]
    float expandedFOV;

    float defaultFOV;


    [Header("References")]

    [SerializeField]
    Camera playerCam;


    // Start is called before the first frame update
    void Start()
    {
        defaultFOV = playerCam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExpandFOV(bool expand)
    {
        float newFOV = expand ? expandedFOV : defaultFOV;

        if (newFOV != playerCam.fieldOfView)
        {
            StopAllCoroutines();
            StartCoroutine(ShiftFOV(newFOV));
        }
    }

    IEnumerator ShiftFOV(float newFOV)
    {
        while(playerCam.fieldOfView != newFOV)
        {
            playerCam.fieldOfView = Mathf.MoveTowards(playerCam.fieldOfView, newFOV, Time.deltaTime * fovTimeScale);
            yield return null;
        }
    }


    private void OnEnable()
    {
        Instance = this;
    }
    private void OnDisable()
    {
        Instance = null;
    }

}
