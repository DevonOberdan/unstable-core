using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    PLAYING,
    PLAYERWON,
    PLAYERLOST
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Action OnGameLost;
    public Action OnGameStarted;

    public event Action<GameState> OnGameStateChanged;

    [SerializeField] EventSystem.GameEvent onGameLost;

    GameState gameState;
    bool paused;

    public bool Paused 
    { 
        get => paused; 
        set 
        {
            paused = value;
            Time.timeScale = paused ? 0 : 1;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public bool AcceptingGameInput => !Paused;
    public GameState State => gameState;


    private void Awake()
    {
        Instance = this;
        Paused = false;

        SetState(GameState.PLAYING);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetState(GameState newState)
    {
        OnGameStateChanged?.Invoke(newState);
    }

    public void DeathEnd()
    {
        SetState(GameState.PLAYERLOST);
        EndGame();
    }

    public void CoreFullEnd()
    {
        SetState(GameState.PLAYERLOST);
        StartCoroutine(EndGameTransition());
    }

    IEnumerator EndGameTransition()
    {
        Explode();
        yield return new WaitForSeconds(5f);
        EndGame();
    }

    private void EndGame()
    {
        OnGameLost();
        onGameLost.Raise();
    }

    //TODO
    void Explode()
    {

    }
}
