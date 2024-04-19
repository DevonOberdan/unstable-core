using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameScreenController : MonoBehaviour
{
    public static event Action<bool> OnPause;

    public static void SetPause(bool pause) => OnPause?.Invoke(pause);

    bool paused;

    GameScreenUI gameScreenUI;

    void Start()
    {
        gameScreenUI = GetComponent<GameScreenUI>();

        OnPause += SetPauseGame;
        //OnPause += (pause) => CursorManager.SetLock(!pause);
        //OnPause += (pause) => FindObjectOfType<Gun>().enabled=!pause;
    }

    void Update()
    {
        CheckMenuInput();
    }

    void CheckMenuInput()
    {
        if ((!paused || (paused && gameScreenUI.PauseMenuOpen)) && Input.GetKeyDown(KeyCode.Escape))
        {
            OnPause.Invoke(!paused);
        }
    }

    void SetPauseGame(bool pause)
    {
        paused = pause;
        Time.timeScale = paused ? 0 : 1;
    }
}
