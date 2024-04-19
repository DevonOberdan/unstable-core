using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIElement
{
    public class SettingsMenuUI : MenuUI
    {
        protected Button closeButton;

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
            closeButton = root.Q<Button>(GetUINameFormat(nameof(closeButton)));
        }

        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();
            closeButton.clicked += Close;
        }
    }
}