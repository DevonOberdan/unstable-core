using ScriptableObjectLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UISystem {

    public class GameOverMenuUI : MenuUI
    {
        Button retryButton;
        Button quitButton;

        public Button RetryButton => retryButton;
        public Button QuitButton => quitButton;

        public Action OnRetry, OnQuit;
        GameOverMenuController gameOverController;

        #region MonoBehaviour Callbacks
        protected override void Awake()
        {
            gameOverController = GetComponent<GameOverMenuController>();
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

            retryButton = root.Q<Button>(GetUINameFormat(nameof(retryButton)));
            quitButton = root.Q<Button>(GetUINameFormat(nameof(quitButton)));
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            retryButton.clicked += () => OnRetry();
            quitButton.clicked += () => OnQuit();
        }
    }
}
