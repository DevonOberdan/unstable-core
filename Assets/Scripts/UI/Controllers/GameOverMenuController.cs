using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UISystem;
using ScriptableObjectLibrary;

public class GameOverMenuController : MonoBehaviour
{
    GameOverMenuUI menuUI;

    SceneMenuManager sceneMenuManager;
    SceneLoadRequester sceneLoadRequester;

    [SerializeField] SceneLoadConfigSO gameScene;
    [SerializeField] SceneLoadConfigSO quitScene;

    [SerializeField] bool scriptableSystem;

    void Start()
    {
        menuUI = GetComponent<GameOverMenuUI>();
        sceneLoadRequester = GetComponent<SceneLoadRequester>();

        menuUI.OnRetry += Retry;
        menuUI.OnQuit += Quit;

        sceneMenuManager = transform.parent.gameObject.GetComponent<SceneMenuManager>();
        //        gameOverMenuUI.OnOpen += sceneMenuManager.HideMenus;

        if(!scriptableSystem)
        {
            Destroy(gameObject.GetComponent<GameEventListener>());
            GameManager.Instance.OnGameLost += () =>
            {
                GameManager.Instance.Paused = true;
                sceneMenuManager.ShowMenu(menuUI);
                //Cursor.lockState = CursorLockMode.None;//lockState ? CursorLockMode.Locked : CursorLockMode.None;
            };
        }


    }

    public void Retry()
    {
        sceneLoadRequester.Request(gameScene); //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit() => sceneLoadRequester.Request(quitScene);
}
