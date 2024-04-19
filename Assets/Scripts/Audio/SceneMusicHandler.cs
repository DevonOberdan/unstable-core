using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusicHandler : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioPlayRequester>().Request();
    }
}
