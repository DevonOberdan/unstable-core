using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace UISystem {

    public class PauseMenuUI : MenuUI
    {
        [SerializeField] MenuUI settingsMenu;

        PauseMenuController pauseMenuController;
        PauseMenuController PauseMenuController => pauseMenuController;// = pauseMenuController != null ? pauseMenuController : GetComponent<PauseMenuController>();

        Button resumeButton, settingsButton, quitButton;

        Volume volume;

        #region MonoBehaviour Callbacks
        protected override void Awake()
        {
            pauseMenuController = GetComponent<PauseMenuController>();
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
            resumeButton = root.Q<Button>(GetUINameFormat(nameof(resumeButton)));

            settingsButton = root.Q<Button>(GetUINameFormat(nameof(settingsButton)));
            quitButton = root.Q<Button>(GetUINameFormat(nameof(quitButton)));

            volume = FindObjectOfType<Volume>();

            //OnOpen += () => BlurBackground(true);
            //OnClose += () => BlurBackground(false);
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            resumeButton.clicked += PauseMenuController.Resume;

            settingsMenu.OnOpen += () => settingsMenu.PreviousMenu = this;
            settingsButton.clicked += settingsMenu.Open;

            quitButton.clicked += PauseMenuController.Quit;
        }
    }
}
