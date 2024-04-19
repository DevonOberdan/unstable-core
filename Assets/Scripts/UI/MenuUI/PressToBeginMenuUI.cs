using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PressToBeginMenuUI : MenuUI
{
    [SerializeField] MenuUI menuToShow;

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
        if (IsVisible && Input.anyKeyDown)
            menuToShow.Open();
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
    }

    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();
        menuToShow.PreviousMenu = this;
    }
}
