using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

[CustomEditor(typeof(DropPodManager))]
public class TransformSpawnEditor : Editor
{
    [SerializeField] GameObject dropPodSpawnPoint;
    DropPodManager dropPodManager;

    protected GameObject spawnedObject;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Spawn Button"))
        {

        }
    }

    private void OnEnable()
    {
        dropPodManager = (DropPodManager)target;
    }

    private void OnSceneGUI()
    {
        if (!Selection.Contains(dropPodManager.gameObject))
        {
            Debug.LogWarning("Need to select DropPodManager to spawn objects");
           // return;
        }


        Event e = Event.current;
        if(e.type == EventType.MouseDown && Event.current.button==0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction*1000, Color.green, 30);
            
            if(Physics.Raycast(ray, out RaycastHit hit, 1000, 1<<LayerMask.NameToLayer("Planet")))
            {
                spawnedObject = Instantiate(dropPodSpawnPoint, hit.point, Quaternion.LookRotation(hit.normal), dropPodManager.transform);
                spawnedObject.name = "SpawnedObject_"+(spawnedObject.transform.GetSiblingIndex()+1);
            }
        }
    }

    void PlaceSpawnPoint(Vector3 point)
    {
        spawnedObject = new GameObject("SpawnPoint");
        spawnedObject.transform.position = point;
        spawnedObject.transform.parent = dropPodManager.transform;
    }
}