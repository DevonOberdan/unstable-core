using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreShells : MonoBehaviour
{
    [SerializeField] Core core;

    RotateObject rotator;

    [SerializeField] bool overrideSlowdownSpeed;

    [DrawIf(nameof(overrideSlowdownSpeed), true)]
    [SerializeField] float slowdownFactor = 0.1f;

    [SerializeField] bool overrideSlowdownTime;

    [DrawIf(nameof(overrideSlowdownTime), true)]
    [SerializeField] float slowdownTimeFactor = 0.1f;

    void OnEnable()
    {
        core.OnRotationSet += SetRotationSpeed;
    }

    private void OnDisable()
    {
        core.OnRotationSet -= SetRotationSpeed;
    }

    private void Start()
    {
        rotator = GetComponent<RotateObject>();
    }

    public void SetRotationSpeed(float newSpeed, float time)
    {
        if(newSpeed < rotator.DampenFactor)
            rotator.SetDampenFactor(overrideSlowdownSpeed ? slowdownFactor : newSpeed,
                                    overrideSlowdownTime  ? slowdownTimeFactor : time);
        else
            rotator.SetDampenFactor(newSpeed, time);
    }


    void Update()
    {
        
    }
}
