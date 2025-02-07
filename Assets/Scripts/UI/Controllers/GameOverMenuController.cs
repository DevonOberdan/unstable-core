using UnityEngine;
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

        if(!scriptableSystem)
        {
            Destroy(gameObject.GetComponent<GameEventListener>());
            GameManager.Instance.OnGameLost += () =>
            {
                GameManager.Instance.Paused = true;
                sceneMenuManager.ShowMenu(menuUI);
            };
        }
    }

    public void Retry()
    {
        sceneLoadRequester.Request(gameScene);
    }

    public void Quit() => sceneLoadRequester.Request(quitScene);
}
