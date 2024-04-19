using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private void Awake()
    {

    }

    private void Start()
    {
        
    }




    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Core"))
        {
            GameManager.Instance.DeathEnd();
        }
    }
}
