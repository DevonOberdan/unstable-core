#if (UNITY_EDITOR)

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class FirstPersonSphereDebug : MonoBehaviour
{
    FirstPersonSphereMovementRB playerMovementScript;

    private void Awake()
    {
        playerMovementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonSphereMovementRB>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PrintMovementStates();
    }

    private void PrintMovementStates()
    {
        ClearConsole();

        if (playerMovementScript.IsGrounded) Debug.Log($"Is Grounded: <color=green>{playerMovementScript.IsGrounded}</color>");
        else Debug.Log($"Is Grounded: <color=red>{playerMovementScript.IsGrounded}</color>");

        if (playerMovementScript.IsInAir) Debug.Log($"Is In Air: <color=green>{playerMovementScript.IsInAir}</color>");
        else Debug.Log($"Is In Air: <color=red>{playerMovementScript.IsInAir}</color>");

        if (playerMovementScript.IsFalling) Debug.Log($"Is Falling: <color=green>{playerMovementScript.IsFalling}</color>");
        else Debug.Log($"Is Falling: <color=red>{playerMovementScript.IsFalling}</color>");

        if (playerMovementScript.IsIdle) Debug.Log($"Is Idle: <color=green>{playerMovementScript.IsIdle}</color>");
        else Debug.Log($"Is Idle: <color=red>{playerMovementScript.IsIdle}</color>");

        if (playerMovementScript.IsSprinting) Debug.Log($"Is Sprinting: <color=green>{playerMovementScript.IsSprinting}</color>");
        else Debug.Log($"Is Sprinting: <color=red>{playerMovementScript.IsSprinting}</color>");

        if (playerMovementScript.IsRunning) Debug.Log($"Is Running: <color=green>{playerMovementScript.IsRunning}</color>");
        else Debug.Log($"Is Running: <color=red>{playerMovementScript.IsRunning}</color>");
    }

    private static void ClearConsole()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type type = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
#endif
