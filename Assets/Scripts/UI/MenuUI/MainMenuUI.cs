using ScriptableObjectLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UISystem
{
    public class MainMenuUI : MenuUI
    {
        public Action OnHitPlay;
        public Action OnHitQuit;

        [Header("Button Menu References")]
        [SerializeField] MenuUI settingsMenu;
        [SerializeField] MenuUI creditsUI;

        Button playButton;
        Button settingsButton;
        Button creditsButton;
        Button quitButton;

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

            playButton      = root.Q<Button>(GetUINameFormat(nameof(playButton)));
            settingsButton  = root.Q<Button>(GetUINameFormat(nameof(settingsButton)));
            creditsButton   = root.Q<Button>(GetUINameFormat(nameof(creditsButton)));
            quitButton      = root.Q<Button>(GetUINameFormat(nameof(quitButton)));
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            playButton.clicked += () => OnHitPlay();

            settingsMenu.OnOpen += () => settingsMenu.PreviousMenu = this;
            creditsUI.OnOpen += () => creditsUI.PreviousMenu = this;

            settingsButton.clicked += settingsMenu.Open;
            creditsButton.clicked += creditsUI.Open;

            quitButton.clicked += () => OnHitQuit();
        }
    }
}