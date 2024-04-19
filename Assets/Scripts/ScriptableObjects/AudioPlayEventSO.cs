using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptableObjectLibrary {

    [CreateAssetMenu(fileName = "AudioPlayEventSO", menuName = "Audio ScriptableObjects/AudioPlayEventSO", order=0)]
    public class AudioPlayEventSO : ScriptableObject
    {
        public Action<AudioConfigSO> OnRequestAudio;

        public void Raise(AudioConfigSO audioConfig)
        {
            OnRequestAudio?.Invoke(audioConfig);    
        }
    }
}
