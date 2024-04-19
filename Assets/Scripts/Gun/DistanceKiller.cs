using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DistanceKiller : MonoBehaviour
{
    readonly float MAX_DISTANCE = 100;

    readonly Vector3 origin = Vector3.zero;

    [SerializeField] UnityEvent OnDistanceReached;

    void Update()
    {
        if (Vector3.Distance(transform.position, origin) > MAX_DISTANCE)
            OnDistanceReached.Invoke();
    }
}
