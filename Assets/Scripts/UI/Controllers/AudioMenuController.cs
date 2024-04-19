using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectLibrary;
using UnityEngine;

namespace UISystem
{
    public class AudioMenuController : MonoBehaviour
    {
        SettingsManager settingsManager;

        AudioMenuUI audioMenuUI;

        void Start()
        {
            settingsManager = FindObjectOfType<SettingsManager>();
            audioMenuUI = GetComponent<AudioMenuUI>();

            audioMenuUI.OnMasterVolumeChanged += MasterValueChanged;
            audioMenuUI.OnMusicVolumeChanged += MusicValueChanged;
            audioMenuUI.OnSfxVolumeChanged += SFXValueChanged;

            SetSliders();
        }

        void SetSliders()
        {
            audioMenuUI.AudioMasterSlider.value = SettingsManager.Audio.MasterVolume;
            audioMenuUI.AudioMusicSlider.value = SettingsManager.Audio.MusicVolume;
            audioMenuUI.AudioSfxSlider.value = SettingsManager.Audio.SfxVolume;
        }

        void MasterValueChanged(float newValue)
        {
            SettingsManager.Audio.MasterVolume = newValue;
        }

        void MusicValueChanged(float newValue)
        {
            SettingsManager.Audio.MusicVolume = newValue;
        }

        void SFXValueChanged(float newValue)
        {
            SettingsManager.Audio.SfxVolume = newValue;
        }
    }
}