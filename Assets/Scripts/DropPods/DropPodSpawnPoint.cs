using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DropPodSpawnPoint : MonoBehaviour
{
    [Range(0.2f, 5)]
    [SerializeField] float radius = 5;

    Transform helperTransform;
    BeamEffect incomingSpawnEffect;

    public DropPod   CurrentDropPod { get; set; }
    public bool      HasDropPod         => CurrentDropPod != null;
    public Transform DropPoint          => helperTransform;
    public Vector3   StartingSpawnPoint => incomingSpawnEffect.transform.position;

    private void Awake()
    {
        helperTransform = new GameObject(nameof(helperTransform)).transform;
        helperTransform.parent = transform;
        helperTransform.localPosition = Vector3.zero;
        helperTransform.localRotation = Quaternion.identity;

        helperTransform.gameObject.AddComponent(typeof(GravitySource));
    }

    public void EndEffect()
    {
        if(incomingSpawnEffect != null)
        {

            incomingSpawnEffect.transform.localRotation = Quaternion.identity;
            incomingSpawnEffect.transform.localPosition = incomingSpawnEffect.transform.localPosition.NewZ(0);
            incomingSpawnEffect.FlipBeam();
            incomingSpawnEffect.EndBeam();
            incomingSpawnEffect = null;
        }
    }

    public void PodSpawnEffect(BeamEffect spawnEffectPrefab)
    {
        incomingSpawnEffect = Instantiate(spawnEffectPrefab, DropPoint.position, Quaternion.identity, DropPoint);

        incomingSpawnEffect.transform.localEulerAngles = new Vector3(0, 180, 0);
        incomingSpawnEffect.transform.localPosition = new Vector3(0, 0, incomingSpawnEffect.transform.GetChild(0).localScale.y * 2);
    }


    public Transform GetNewDropPoint()
    {
        Vector2 offset = Random.insideUnitCircle * radius;

        helperTransform.localPosition = new Vector3(offset.x, offset.y, 0);

        if (Physics.Raycast(helperTransform.position, -transform.forward, out RaycastHit hit, 10, ~LayerMask.NameToLayer("Planet")))
        {
            Debug.DrawRay(helperTransform.position, -transform.forward, Color.magenta, 5f);
            helperTransform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
            Debug.DrawRay(hit.point, hit.normal, Color.cyan, 5f);
        }

        return helperTransform;
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.forward, radius, 3.0f);
#endif
    }
}
