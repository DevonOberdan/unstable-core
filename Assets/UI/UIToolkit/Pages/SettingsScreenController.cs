using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreenController : MonoBehaviour
{
    public static Action<Settings> OnAudioSettingsChanged;

    [SerializeField] SettingsManager settingsManager;

    //void Start()
    //{
    //    SettingsScreen.OnMasterVolumeChanged += SetMasterVolume;
    //    SettingsScreen.OnMusicVolumeChanged += SetMusicVolume;
    //    SettingsScreen.OnSfxVolumeChanged += SetSfxVolume;

    //    OnAudioSettingsChanged(settingsManager.Settings);
    //}

    //void SetMasterVolume(float newMasterVolume)
    //{
    //    settingsManager.Settings.MasterVolume = newMasterVolume;
    //    OnAudioSettingsChanged(settingsManager.Settings);
    //}

    //void SetMusicVolume(float newMusicVolume)
    //{
    //    settingsManager.Settings.MusicVolume = newMusicVolume;
    //    OnAudioSettingsChanged(settingsManager.Settings);
    //}

    //void SetSfxVolume(float newSfxVolume)
    //{
    //    settingsManager.Settings.SfxVolume = newSfxVolume;
    //    OnAudioSettingsChanged(settingsManager.Settings);
    //}



    void Update()
    {
        
    }
}