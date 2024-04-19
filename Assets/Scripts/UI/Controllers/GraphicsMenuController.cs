using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
using System;
using System.Linq;
using UnityEngine.Rendering;

public class GraphicsMenuController : MonoBehaviour
{
    SettingsManager settingsManager;
    GraphicsMenuUI menuUI;

    List<FullScreenMode> allModes;
    List<ResolutionData> allResolutions;
    List<int> allFramerates;

    //List<string> allPresets;// = new string[4] { "Low", "Medium", "High", "Custom" };

    [SerializeField] bool limitedResolutions;


    void Start()
    {
        settingsManager = FindObjectOfType<SettingsManager>();
        menuUI = GetComponent<GraphicsMenuUI>();

        ProcessPresets();

        ProcessResolutions();
        ProcessFramerate();
        ProcessScreenMode();
       // ProcessVSync();

        menuUI.OnSettingsChanged += CheckForChangesToApply;
        menuUI.OnPresetChanged   += CheckForChangesToApply;

        menuUI.OnPresetValueChanged += SetPresetToCustom;

        menuUI.OnPresetChanged += HandlePresetChanged;

        menuUI.OnApplySettings += ApplySettings;
    }

    //TODO: figure out how to change all related settings as soon as the preset is switched in the UI
    // while also ensuring that they dont trigger their change event that has other ramifications, etc and so on
    void HandlePresetChanged()
    {
        // we are on Custom, which has been added last
        if (menuUI.CurrentPresetIndex == QualitySettings.names.Length-1)
        {
           // QualitySettings.
            // set nothing
        }
        else
        {
            menuUI.RemoveCustomPresetOption();
            //RenderPipelineAsset asset = QualitySettings.GetRenderPipelineAssetAt(menuUI.CurrentPresetIndex);
            if(menuUI.CurrentPresetIndex != QualitySettings.GetQualityLevel())
                QualitySettings.SetQualityLevel(menuUI.CurrentPresetIndex, false);

            menuUI.SetCurrentVSyncValue(QualitySettings.vSyncCount > 0, false);
            // set all graphics options, without notifying
        }

        // if non preset value is changed while it is not listed as custom, then it needs to be changed to custom
    }




    void ProcessPresets()
    {
        menuUI.PopulatePresetOptions(QualitySettings.names.SkipLast(1).ToList());//new string[3] { "Low", "Medium", "High" }.ToList());
        if (QualitySettings.GetQualityLevel() >= menuUI.QualityPresetOptions.Count)
            menuUI.AddAndSetToCustomPreset();

        string startingPreset = menuUI.QualityPresetOptions[QualitySettings.GetQualityLevel()];
        menuUI.SetCurrentPresetValue(startingPreset);
        HandlePresetChanged();
    }

    void ProcessResolutions()
    {
        // send all options to MenuUI
        allResolutions = GetAllResolutions();

        menuUI.PopulateResolutionOptions(allResolutions.Select(res => res.ToString()).ToList());

        // send defaults to MenuUI from Settings
        string currentRes = SettingsManager.Graphics.Resolution.ToString();
        menuUI.SetCurrentResolution(currentRes);
    }

    void ProcessFramerate()
    {
        allFramerates = GetAllFramerates();
        menuUI.PopulateFramerateOptions(allFramerates.Select(fr => fr.ToString()).ToList());
        menuUI.SetCurrentFramerateValue(SettingsManager.Graphics.FramerateTarget.ToString());
    }

    void ProcessScreenMode()
    {
        allModes = ((IEnumerable<FullScreenMode>)(Enum.GetValues(typeof(FullScreenMode)))).ToList();

        menuUI.PopulateScreenOptions(allModes.Select(mode => mode.ToString()).ToList());

        string currentMode = SettingsManager.Graphics.ScreenMode.ToString();
        menuUI.SetCurrentScreenModeValue(currentMode);
    }


    void ProcessVSync()
    {
        menuUI.SetCurrentVSyncValue(SettingsManager.Graphics.VSyncOn);
    }

    private List<ResolutionData> GetAllResolutions()
    {
        List<ResolutionData> resolutions = new();

        foreach (Resolution screenRes in Screen.resolutions)
        {
            if (NewResolution(screenRes))
            {
                ResolutionData newRes = new ResolutionData(screenRes.width, screenRes.height);
                resolutions.Add(newRes);
            }
        }
        return resolutions;


        bool NewResolution(Resolution screenRes)
        {
            return resolutions.FirstOrDefault(res => res.width == screenRes.width && res.height == screenRes.height) == null;
        }
    }

    private List<int> GetAllFramerates()
    {
        List<int> framerates = new();

        foreach(Resolution res in Screen.resolutions)
        {
            if (!framerates.Contains(res.refreshRate))
            {
                framerates.Add(res.refreshRate);
            }
        }

        // backup list for common, default framerates if device only lists one framerate
        if(framerates.Count <= 1)
        {
            framerates.Clear();
            framerates = SettingsManager.Graphics.FRAMERATES.ToList();
        }

        return framerates;
    }

    void ApplySettings()
    {
        ApplyResolution();
        ApplyFramerate();
        ApplyScreenMode();

        ApplyPreset();

        //ApplyVSync();
        menuUI.SetApplyEnabled(false);
    }


    void CheckForChangesToApply()
    {
        bool presetChanged = menuUI.CurrentPresetIndex != SettingsManager.Graphics.QualityPresetLevel;

        bool resolutionChanged = menuUI.SelectedResolution != SettingsManager.Graphics.Resolution.ToString();
        bool framerateChanged = menuUI.SelectedFramerate != SettingsManager.Graphics.FramerateTarget;
        bool modeChanged = menuUI.ScreenModeIndex != (int)SettingsManager.Graphics.ScreenMode;
        bool vSyncChanged = menuUI.VSyncToggled != SettingsManager.Graphics.VSyncOn;

        menuUI.SetApplyEnabled(presetChanged || resolutionChanged || modeChanged || framerateChanged || vSyncChanged);
    }

    void SetPresetToCustom()
    {
        menuUI.AddAndSetToCustomPreset();

        //if (allPresets[menuUI.CurrentPresetIndex].Equals("Custom"))
        //{
        //    // set nothing
        //}
        //else
        //{
        //    // can I get the vSync/antiAliasing level/ etc. of a level that I am not currently on??
        //    RenderPipelineAsset asset = QualitySettings.GetRenderPipelineAssetAt(menuUI.CurrentPresetIndex);
        //}
        //SettingsManager.Graphics.QualityPresetLevel = 3;
        //menuUI.SetPreset(3);//SettingsManager.Graphics.QualityPresetLevel);
    }


    public void ApplyResolution()
    {
        SettingsManager.Graphics.SetResolution(menuUI.SelectedResolution);
    }

    public void ApplyFramerate()
    {
        SettingsManager.Graphics.FramerateTarget = menuUI.SelectedFramerate;
    }


    public void ApplyScreenMode()
    {
        FullScreenMode selectedMode = (FullScreenMode)menuUI.ScreenModeIndex;
        SettingsManager.Graphics.ScreenMode = selectedMode;
    }

    public void ApplyPreset()
    {
        SettingsManager.Graphics.QualityPresetLevel = menuUI.CurrentPresetIndex;
    }

    public void ApplyVSync() => SettingsManager.Graphics.VSyncOn = menuUI.VSyncToggled;

    private void OnDisable()
    {
        menuUI.OnApplySettings -= ApplySettings;
        menuUI.OnSettingsChanged -= CheckForChangesToApply;
    }
}
