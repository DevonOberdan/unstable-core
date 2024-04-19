using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioSettingsMenuUI : MenuUI 
{
    public Action<float> OnMasterVolumeChanged;
    public Action<float> OnMusicVolumeChanged;
    public Action<float> OnSfxVolumeChanged;

    public static Action OnReturn;

    //public static Action OnClose;


    [Header("Menu Screen elements")]
    [Tooltip("String IDs to query Visual Elements")]
   // [SerializeField] string settingsScreenName = "SettingsScreen";

    Slider audioMasterSlider;
    Slider audioMusicSlider;
    Slider audioSfxSlider;

    Button closeButton;
    //string closeButtonName = "close-button";


    // automate string typing/reference getting by keeping a consistent
    // naming convention of:
    // XML: x-y-z
    // variable: xYZName
    // parse/split nameof(variable), lowercase it all, then combine to get uxml name
    //string audioMasterSliderName = "audio-master-slider";
    //string audioMusicSliderName = "audio-music-slider";
    //string audioSfxSliderName = "audio-sfx-slider";


    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        base.Awake();
        //OnClose += () => sceneMenuManager.CloseMenu(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
    #endregion

    protected override void GrabVisualElements()
    {
        base.GrabVisualElements();

        closeButton = root.Q<Button>(GetUINameFormat(nameof(closeButton)));
        print(closeButton);
        audioMasterSlider = root.Q<Slider>(GetUINameFormat(nameof(audioMasterSlider)));
        audioMusicSlider  = root.Q<Slider>(GetUINameFormat(nameof(audioMusicSlider)));
        audioSfxSlider    = root.Q<Slider>(GetUINameFormat(nameof(audioSfxSlider)));

        //audioMasterSlider = root.Q<Slider>(elementPrefix + audioMasterSliderName);
        //audioMusicSlider = root.Q<Slider>(elementPrefix + audioMusicSliderName);
        //audioSfxSlider = root.Q<Slider>(elementPrefix + audioSfxSliderName);
    }

    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();

        closeButton.clicked += Close;

        audioMasterSlider?.RegisterValueChangedCallback(SetMasterVolume);
        audioMusicSlider?.RegisterValueChangedCallback(SetMusicVolume);
        audioSfxSlider?.RegisterValueChangedCallback(SetSfxVolume);
    }

    void SetMasterVolume(ChangeEvent<float> evt) => OnMasterVolumeChanged(evt.newValue);
    void SetMusicVolume(ChangeEvent<float> evt) => OnMusicVolumeChanged(evt.newValue);
    void SetSfxVolume(ChangeEvent<float> evt) => OnSfxVolumeChanged(evt.newValue);
}