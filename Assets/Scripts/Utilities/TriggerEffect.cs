using UnityEngine;
using UnityEngine.Events;

public class TriggerEffect : MonoBehaviour
{
    public UnityEvent OnTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTriggered.Invoke();
        }
    }
}
