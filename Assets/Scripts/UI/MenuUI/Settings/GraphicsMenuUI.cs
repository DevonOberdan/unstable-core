using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using MyUILibrary;
namespace UISystem {

    public class GraphicsMenuUI : MenuUI
    {
        DropdownField resolutionDropdown;

        CarouselSelector framerateSelector;
        CarouselSelector screenModeSelector;

        CarouselSelector qualityPresetSelector;

        SlideToggle vsyncToggle;

        Button applyButton;

        const string CUSTOM_PRESET_NAME = "Custom";

        public DropdownField ResolutionDropdown => resolutionDropdown;

        public int CurrentPresetIndex => qualityPresetSelector.index;
        public string SelectedResolution => ResolutionDropdown.value;
        public int SelectedFramerate => int.Parse(framerateSelector.value);
        public int ScreenModeIndex => screenModeSelector.index;
        public bool VSyncToggled => vsyncToggle.value;

        public List<string> QualityPresetOptions => qualityPresetSelector.choices;
        public bool ContainsCustomQualityPreset => qualityPresetSelector.choices.Contains(CUSTOM_PRESET_NAME);

        public Action OnSettingsChanged;
        public Action OnApplySettings;

        public Action OnPresetValueChanged;
        public Action OnPresetChanged;

        #region MonoBehaviour Callbacks
        protected override void Awake()
        {
            base.Awake();
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
            resolutionDropdown = root.Q<DropdownField>(GetUINameFormat(nameof(resolutionDropdown)));

            if (resolutionDropdown.choices == null)
                resolutionDropdown.choices = new();

            resolutionDropdown.choices.Clear();

            qualityPresetSelector = root.Q<CarouselSelector>(GetUINameFormat(nameof(qualityPresetSelector)));

            framerateSelector = root.Q<CarouselSelector>(GetUINameFormat(nameof(framerateSelector)));

            screenModeSelector = root.Q<CarouselSelector>(GetUINameFormat(nameof(screenModeSelector)));

            vsyncToggle = root.Q<SlideToggle>(GetUINameFormat(nameof(vsyncToggle)));

            applyButton = root.Q<Button>(GetUINameFormat(nameof(applyButton)));
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            DisplayVisualElement(applyButton, true);
            SetApplyEnabled(false);
            applyButton.clicked += () => OnApplySettings();

            qualityPresetSelector.RegisterValueChangedCallback(evt => OnPresetChanged?.Invoke());

            ListenForSettingsChanges();
            ListenForPresetValueChanges();
        }


        void ListenForSettingsChanges()
        {
           // qualityPresetSelector.RegisterValueChangedCallback(evt => OnSettingsChanged?.Invoke());

            ResolutionDropdown.RegisterValueChangedCallback(evt => OnSettingsChanged?.Invoke());

            framerateSelector.RegisterValueChangedCallback(evt => OnSettingsChanged?.Invoke());
            screenModeSelector.RegisterValueChangedCallback(evt => OnSettingsChanged?.Invoke());

        }

        void ListenForPresetValueChanges()
        {
            vsyncToggle.RegisterValueChangedCallback(evt => OnPresetValueChanged?.Invoke());
        }

        public void SetApplyEnabled(bool enable) => applyButton.SetEnabled(enable);

        public void SetPreset(int presetIndex)
        {
            qualityPresetSelector.index = presetIndex;
        }

        public void PopulateResolutionOptions(List<string> options) => ResolutionDropdown.choices = options;
        public void PopulateFramerateOptions(List<string> options) => framerateSelector.choices = options;
        public void PopulateScreenOptions(List<string> options) => screenModeSelector.choices = options;

        public void PopulatePresetOptions(List<string> options) => qualityPresetSelector.choices = options;
        
        public void AddAndSetToCustomPreset()
        {
            if(!ContainsCustomQualityPreset)
                qualityPresetSelector.choices.Add(CUSTOM_PRESET_NAME);

            SetCurrentPresetValue(CUSTOM_PRESET_NAME);
        }

        public void RemoveCustomPresetOption()
        {
            if (ContainsCustomQualityPreset)
                qualityPresetSelector.choices.Remove(CUSTOM_PRESET_NAME);
        }

        public void SetCurrentResolution(string value, bool notify = true)
        {
            if (notify)
                ResolutionDropdown.value = value;
            else
                ResolutionDropdown.SetValueWithoutNotify(value);
        }

        public void SetCurrentScreenModeValue(string value, bool notify = true)
        {
            if(notify)
                screenModeSelector.value = value;
            else
                screenModeSelector.SetValueWithoutNotify(value);
        }
        public void SetCurrentFramerateValue(string value, bool notify = true)
        {
            if(notify)
                framerateSelector.value = value;
            else
                framerateSelector.SetValueWithoutNotify(value);
        }

        public void SetCurrentPresetValue(string value, bool notify = true)
        {
            if(notify)
                qualityPresetSelector.value = value;
            else
                qualityPresetSelector.SetValueWithoutNotify(value);
        }

        public void SetCurrentVSyncValue(bool sync, bool notify=true)
        {
            if(notify)
                vsyncToggle.value = sync;
            else
                vsyncToggle.SetValueWithoutNotify(sync);
        }
    }
}
