using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ResolutionData
{
    public int width, height;

    public ResolutionData(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public ResolutionData(Resolution resolution)
    {
        this.width = resolution.width;
        this.height = resolution.height;
    }

    public override string ToString() => width + " x " + height;
}

public class SettingsGraphics : Settings
{
    const FullScreenMode DEFAULT_MODE = FullScreenMode.ExclusiveFullScreen;
    const int DEFAULT_FRAMERATE = 60;
    const int DEFAULT_VSYNC_VALUE = 0;
    const int DEFAULT_PRESET = 2;

    public readonly int[] FRAMERATES = new int[] { 5, 30, 60, 90, 120, 144 };

    ResolutionData DEFAULT_RESOLUTION;

    FullScreenMode screenMode;
    ResolutionData resolution;
    int framerateTarget;

    bool vSyncOn;

    // NOT references to each one, but shifting levels instead
    [SerializeField] RenderPipelineAsset qualityPresetLow;
    [SerializeField] RenderPipelineAsset qualityPresetMedium;
    [SerializeField] RenderPipelineAsset qualityPresetHigh;
    [SerializeField] RenderPipelineAsset qualityPresetUltra;
    [SerializeField] RenderPipelineAsset qualityPresetCustom;

    RenderPipelineAsset qualityCurrentPreset;

    public FullScreenMode ScreenMode
    {
        get => screenMode;
        set 
        {
            screenMode = value;
            Screen.fullScreenMode = screenMode;
        }
    }

    public ResolutionData Resolution 
    {
        get => resolution;
        set 
        {
            resolution = value;
            Screen.SetResolution(resolution.width, resolution.height, ScreenMode);
        }
    }

    public int FramerateTarget
    {
        get => framerateTarget;
        set
        {
            framerateTarget = value;
            Application.targetFrameRate = framerateTarget;
        }
    }

    public bool VSyncOn 
        {
        get => vSyncOn;
        set {
            vSyncOn = value;
            //QualitySettings.vSyncCount = vSyncOn ? 1 : 0;
        }
    }

    int textureDetail;
    public int TextureDetail 
    {
        get => textureDetail;
        set {
            textureDetail = value;
        }
    }

    int shadowDetail;
    public int ShadowDetail {
        get => shadowDetail;
        set
        {
            shadowDetail = value;
        }
    }

    // via renderpipelineasset
    bool ambientOcclusionActive;
    public bool AmbientOcclusionActive
    {
        get => ambientOcclusionActive;
        set
        {
            ambientOcclusionActive = value;
        }
    }

    //on or off
    int antiAliasingLevel;
    public int AntiAliasingLevel
    {
        get => antiAliasingLevel;
        set
        {
            antiAliasingLevel = value;
            QualitySettings.antiAliasing = antiAliasingLevel;
        }
    }

    // off, on, or force enable
    int anisotropicFilterLevel;
    public int AnisotropicFilterLevel
    {
        get => AnisotropicFilterLevel;
        set
        {
            int newValue = Mathf.Clamp(value, 0, Enum.GetNames(typeof(AnisotropicFiltering)).Length);
            anisotropicFilterLevel = newValue;
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)anisotropicFilterLevel;
        }
    }
    public void SetAnisotropicFilterLevel()
    {

    }

    // set FOV here??
    // quality level (l,m,h, custom)
    int qualityPresetLevel;
    public int QualityPresetLevel {
        get => qualityPresetLevel;
        set {
            qualityPresetLevel = value;
            QualitySettings.SetQualityLevel(qualityPresetLevel, true);

            // if custom, do stuff
        }
    }


    public bool FullScreen => ScreenMode == FullScreenMode.ExclusiveFullScreen;

    public void SetResolution(string newRes) => Resolution = ExtractResData(newRes);


    ResolutionData ExtractResData(string resStr)
    {
        string[] resArr = resStr.Split('x');
        int width = int.Parse(resArr[0].Trim());
        int height = int.Parse(resArr[1].Trim());
        return new ResolutionData(width, height);
    }

    public override void Load()
    {
        if (DEFAULT_RESOLUTION == null)
            DEFAULT_RESOLUTION = new ResolutionData(Screen.currentResolution);

        QualityPresetLevel = PlayerPrefs.GetInt(nameof(QualityPresetLevel), DEFAULT_PRESET);

        ScreenMode = (FullScreenMode)PlayerPrefs.GetInt(nameof(ScreenMode), (int)DEFAULT_MODE);
        Resolution = ExtractResData(PlayerPrefs.GetString(nameof(Resolution), DEFAULT_RESOLUTION.ToString()));
        FramerateTarget = PlayerPrefs.GetInt(nameof(FramerateTarget), DEFAULT_FRAMERATE);

        VSyncOn = QualitySettings.vSyncCount > 0; //PlayerPrefs.GetInt(nameof(VSyncOn), DEFAULT_VSYNC_VALUE) == 1;
    }

    public override void Reset()
    {
        ScreenMode = DEFAULT_MODE;
        FramerateTarget = DEFAULT_FRAMERATE;
        VSyncOn = DEFAULT_VSYNC_VALUE != 0;
        base.Reset();
    }

    public override void Save()
    {
        PlayerPrefs.SetInt(nameof(QualityPresetLevel), QualityPresetLevel);

        PlayerPrefs.SetInt(nameof(ScreenMode), (int)ScreenMode);
        PlayerPrefs.SetInt(nameof(FramerateTarget), FramerateTarget);
        PlayerPrefs.SetString(nameof(Resolution), Resolution.ToString());
        //PlayerPrefs.SetInt(nameof(VSyncOn), QualitySettings.vSyncCount);

        PlayerPrefs.SetInt(nameof(AntiAliasingLevel), AntiAliasingLevel);

        base.Save();
    }
}
