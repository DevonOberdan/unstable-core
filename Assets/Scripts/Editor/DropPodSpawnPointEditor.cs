using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DropPodSpawnPoint))]
public class DropPodSpawnPointEditor : Editor
{
    [SerializeField] GameObject dropPodPrefab;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Spawn Drop Pod"))
        {
            DropPodSpawnPoint spawnPoint = (DropPodSpawnPoint)target;

            if(spawnPoint.transform.parent.TryGetComponent(out DropPodManager manager))
            {
                manager.SpawnAtPoint(spawnPoint);
            }
        }
    }
}
