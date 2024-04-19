using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EventSystem;
public class GameEventListener : MonoBehaviour
{
    [SerializeField] EventSystem.GameEvent Event;
    [SerializeField] UnityEvent Response;

    private void Awake()
    {
        Event.RegisterListener(this);
    }

    private void OnDestroy()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventInvoked() => Response?.Invoke();
}
