using System.Collections;
using System.Collections.Generic;
using ScriptableObjectLibrary;
using UnityEngine;

public class AudioPlayRequester : MonoBehaviour
{
    [SerializeField] AudioConfigSO defaultAudio;
    [SerializeField] AudioPlayEventSO audioPlayEvent;

    public void Request(AudioConfigSO requestedConfig=null)
    {
        if (requestedConfig != null)
            audioPlayEvent.Raise(requestedConfig);
        else
            audioPlayEvent.Raise(defaultAudio);
    }
}
