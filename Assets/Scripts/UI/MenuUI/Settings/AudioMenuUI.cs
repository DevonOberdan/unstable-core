using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem
{
    public class AudioMenuUI : MenuUI
    {
        public Action<float> OnMasterVolumeChanged;
        public Action<float> OnMusicVolumeChanged;
        public Action<float> OnSfxVolumeChanged;

        public static Action OnReturn;

        Slider audioMasterSlider;
        Slider audioMusicSlider;
        Slider audioSfxSlider;

        public Slider AudioMasterSlider => audioMasterSlider;
        public Slider AudioMusicSlider => audioMusicSlider;
        public Slider AudioSfxSlider => audioSfxSlider;

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

            audioMasterSlider = root.Q<Slider>(GetUINameFormat(nameof(audioMasterSlider)));
            audioMusicSlider = root.Q<Slider>(GetUINameFormat(nameof(audioMusicSlider)));
            audioSfxSlider = root.Q<Slider>(GetUINameFormat(nameof(audioSfxSlider)));
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
            audioMasterSlider?.RegisterValueChangedCallback(evt => OnMasterVolumeChanged(evt.newValue));
            audioMusicSlider?.RegisterValueChangedCallback(evt => OnMusicVolumeChanged(evt.newValue));
            audioSfxSlider?.RegisterValueChangedCallback(evt => OnSfxVolumeChanged(evt.newValue));
        }
    }
}
