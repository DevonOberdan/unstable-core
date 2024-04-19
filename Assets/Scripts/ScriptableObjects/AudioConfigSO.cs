using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptableObjectLibrary {

    /* As you make more and more of these, if patterns emerge, like one shot type sound effects
     * all having virtually the same Config settings, then maybe make an inherited class that
     * has those defaults, like:
     * 
     * PickupAudioConfigSO
     *      or
     * OneShotAudioConfigSO
     * 
     * first will probably be a OneShot one and a Music one
     * 
     */

    [CreateAssetMenu(fileName = "AudioConfigSO", menuName = "Audio ScriptableObjects/AudioConfigSO", order=0)]
    public class AudioConfigSO : DataConfigSO
    {
        [SerializeField] AudioClip clip;
        [SerializeField] bool oneShot;
        [SerializeField] bool randomPitch;

        public AudioClip Clip => clip;
        public bool OneShot => oneShot;
        public bool RandomPitch => randomPitch;

    }
}
