using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectLibrary;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    static SettingsAudio audioSettings;
    static SettingsGraphics graphicsSettings;

    public static SettingsAudio Audio { get => audioSettings; set => audioSettings = value; }
    public static SettingsGraphics Graphics { get => graphicsSettings; set => graphicsSettings = value; }

    [SerializeField] bool startDefault;

    [Header("Graphics Settings")]
    [SerializeField] int customRendererIndex = 3;
    
    void Awake()
    {
        Audio = new SettingsAudio();
        Graphics = new SettingsGraphics();
    }

    private void OnEnable()
    {
        if (startDefault)
        {
            Audio.Reset();
            Graphics.Reset();
        }
        else
        {
            Audio.Load();
            Graphics.Load();
        }
    }

    private void OnDisable()
    {
        Audio.Save();
        Graphics.Save();
    }


    public void SetGraphicsToCustm()
    {
        Graphics.QualityPresetLevel = customRendererIndex;
    }
}
