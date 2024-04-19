using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public Action<bool> OnSetPause;

    MenuUI pauseMenuUI;

    SceneLoadRequester loadRequester;

   // bool paused;

    void Start()
    {
        pauseMenuUI = GetComponent<MenuUI>();
        loadRequester = GetComponent<SceneLoadRequester>();

        OnSetPause += pauseMenuUI.SetOpen;

        OnSetPause += paused => GameManager.Instance.Paused = paused;
        //OnSetPause += paused => CursorManager.SetLock(!paused);
    }

    void Update()
    {
        if ((!GameManager.Instance.Paused || pauseMenuUI.IsVisible) && Input.GetKeyDown(KeyCode.Escape))
        {
            print("PAUSED!");
            OnSetPause(!GameManager.Instance.Paused);
        }
    }

    public void Resume()
    {
        OnSetPause(false);
    }

    //public void PauseGame(bool pauseState)
    //{
    //    GameManager.Instance.Paused = pauseState;
    //}


    public void Quit() => loadRequester.Request();
}
