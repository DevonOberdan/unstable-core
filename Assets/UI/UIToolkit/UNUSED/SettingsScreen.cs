using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScreen : MonoBehaviour
{
    //public static Action<bool>

    public static Action<float> OnMasterVolumeChanged;
    public static Action<float> OnMusicVolumeChanged;
    public static Action<float> OnSfxVolumeChanged;

    public static Action OnReturn;

    [Header("Menu Screen elements")]
    [Tooltip("String IDs to query Visual Elements")]
    [SerializeField] string settingsScreenName = "SettingsScreen";

    GameScreenUI gameScreenUI;

    UIDocument screenDoc;

    VisualElement rootElement;

    const string PREFIX = "settings-";


    Slider audioMasterSlider;
    Slider audioMusicSlider;
    Slider audioSfxSlider;

    Button closeButton;
    string closeButtonName = "close-button";
    

    // automate string typing/reference getting by keeping a consistent
    // naming convention of:
    // XML: x-y-z
    // variable: xYZName
    // parse/split nameof(variable), lowercase it all, then combine to get uxml name
    string audioMasterSliderName = "audio-master-slider";
    string audioMusicSliderName  = "audio-music-slider";
    string audioSfxSliderName    = "audio-sfx-slider";

    void Start()
    {
        gameScreenUI = GetComponent<GameScreenUI>();

        GrabVisualElements();
        RegisterButtonCallbacks();
    }

    void GrabVisualElements()
    {
        screenDoc = GetComponent<UIDocument>();

        rootElement = screenDoc.rootVisualElement.Q(settingsScreenName);

        closeButton = rootElement.Q<Button>(PREFIX + closeButtonName);

        audioMasterSlider = rootElement.Q<Slider>(PREFIX + audioMasterSliderName);
        audioMusicSlider = rootElement.Q<Slider>(PREFIX + audioMusicSliderName);
        audioSfxSlider = rootElement.Q<Slider>(PREFIX + audioSfxSliderName);
    }

    void RegisterButtonCallbacks()
    {
        closeButton?.RegisterCallback<ClickEvent>((evt) => OnReturn?.Invoke());

        audioMasterSlider?.RegisterValueChangedCallback(SetMasterVolume);
        audioMusicSlider?.RegisterValueChangedCallback(SetMusicVolume);
        audioSfxSlider?.RegisterValueChangedCallback(SetSfxVolume);
    }

    void SetMasterVolume(ChangeEvent<float> evt) => OnMasterVolumeChanged(evt.newValue);
    void SetMusicVolume(ChangeEvent<float> evt) => OnMusicVolumeChanged(evt.newValue);
    void SetSfxVolume(ChangeEvent<float> evt) => OnSfxVolumeChanged(evt.newValue);

    public void ShowScreen()
    {

    }
}
