using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem {

    public class SettingsPageMenuUI : MenuUI
    {

        [SerializeField] StyleSheet menuStyling;

        Button applyButton;

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
            applyButton = root.Q<Button>(GetUINameFormat(nameof(applyButton)));

            root.styleSheets.Add(menuStyling);

            ShowApplyButton(false);

        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
        }


        void DisableApplyButton()
        {
            applyButton.AddToClassList(".button-disabled");
            //applyButton.SetEnabled
        }

        void EnableApplyButton()
        {
            applyButton.RemoveFromClassList(".button-disabled");
        }

        void ShowApplyButton(bool show)
        {
            DisplayVisualElement(applyButton, show);
        }
    }
}
