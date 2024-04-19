/*
 * Attach to empty object and parent with other managers
 */
 
using UnityEngine;

public class CursorManager : MonoBehaviour {

    [SerializeField] bool startLocked;
    //[SerializeField] GameEvent onGameLost;

    void Awake()
    {
        //SetLock(startLocked);
    }

    private void Start()
    {
        //onGameLost.OnEvent += () => SetLock(false);
        //GameManager.Instance.OnGameLost += () => SetLock(false);
    }

    private void OnDestroy()
    {
       // onGameLost.OnEvent -= () => SetLock(false);
        // GameManager.Instance.OnGameLost -= () => SetLock(false);
    }

    //call to set cursor state to confined or locked
    public void SetLock(bool lockState)
    {
        Cursor.lockState = lockState ? CursorLockMode.Locked : CursorLockMode.None;
    }

}
