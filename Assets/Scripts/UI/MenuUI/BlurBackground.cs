using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class BlurBackground : MonoBehaviour
{
    MenuUI menuUI;
    Volume volume;

    private void Awake()
    {
        menuUI = GetComponent<MenuUI>();
        volume = FindObjectOfType<Volume>();
    }

    private void OnEnable()
    {
        menuUI.OnOpen  += () => SetBlur(true);
        menuUI.OnClose += () => SetBlur(false);
    }

    private void OnDisable()
    {
        menuUI.OnOpen  -= () => SetBlur(true);
        menuUI.OnClose -= () => SetBlur(false);
    }

    void SetBlur(bool state)
    {
        if (volume == null)
            return;

        DepthOfField blurDOF;

        if (volume.profile.TryGet<DepthOfField>(out blurDOF))
        {
            blurDOF.active = state;
        }
    }
}
