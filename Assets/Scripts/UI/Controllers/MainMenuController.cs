using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using ScriptableObjectLibrary;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    SceneLoadRequester sceneLoadRequester;
    MainMenuUI mainMenuUI;

    void Start()
    {
        mainMenuUI = GetComponent<MainMenuUI>();
        mainMenuUI.OnHitPlay += Play;
        mainMenuUI.OnHitQuit += QuitGame;
        sceneLoadRequester = GetComponent<SceneLoadRequester>();
    }

    public void Play()
    {
        sceneLoadRequester.Request();
    }

    public void QuitGame()
    {
        print("quitting");
        Application.Quit();
    }
}
