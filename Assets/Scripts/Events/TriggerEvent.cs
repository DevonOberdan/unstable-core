using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent TriggerEnterEvent;
    public UnityEvent TriggerExitEvent;

    private void OnTriggerEnter(Collider other) => TriggerEnterEvent.Invoke();
    private void OnTriggerExit(Collider other) => TriggerExitEvent.Invoke();
}
