using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewGameManager", menuName = "New Game Manager", order =2)]
public class GameManagerTest : ScriptableObject
{
    public Action OnGameLost;

    public bool AcceptingGameInput => !Paused;

    public bool paused;
    public bool Paused {
        get => paused;
        set {
            paused = value;
            Time.timeScale = paused ? 0 : 1;
            //CursorManager.SetLock(!paused);
        }
    }

    public void DeathEnd()
    {
        EndGame();
    }

    public void CoreFullEnd()
    {
        EndGame();//StartCoroutine(EndGameTransition());
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
    }

    void Explode()
    {

    }
}
