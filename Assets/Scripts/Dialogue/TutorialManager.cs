using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{

    public UnityEvent OnPodsIntroduced;

    [SerializeField] Collider firstPodTrigger;


    void Start()
    {
        
    }
}
