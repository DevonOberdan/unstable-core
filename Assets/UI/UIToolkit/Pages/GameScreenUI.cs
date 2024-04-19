using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameScreenUI : MonoBehaviour
{
    UIDocument gameScreenDoc;

    VisualElement menuPause;
    VisualElement inGameUI, menuSettings;

    [SerializeField] Volume volume;


    [Header("Menu Screen elements")] [Tooltip("String IDs to query Visual Elements")]
    [SerializeField] string pauseScreenName = "PauseScreen";
    [SerializeField] string inGameUIName = "InventoryUI";
    [SerializeField] string nameSettingsUI = "SettingsScreen";



    Button resumeButton;
    Button settingsButton;
    Button quitButton;

    string resumeButtonName = "pause-screen__resume-button";
    string settingsButtonName = "pause-screen__settings-button";
    string quitButtonName = "pause-screen__quit-button";

    public bool PauseMenuOpen => menuPause.style.display == DisplayStyle.Flex;

    private void OnEnable()
    {
        GrabVisualElements();
        RegisterButtonCallbacks();

        GameScreenController.OnPause += SetPauseScreen;
        SettingsScreen.OnReturn += () => SetSettingsScreen(false);
    }

    void GrabVisualElements()
    {
        gameScreenDoc = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreenDoc.rootVisualElement;
        
        //pauseMenu = rootElement;
        //inGameUI = inventoryDoc.rootVisualElement;
        menuPause =  rootElement.Q(pauseScreenName);
        inGameUI = rootElement.Q(inGameUIName);
        menuSettings = rootElement.Q(nameSettingsUI);

        resumeButton = menuPause.Q<Button>(resumeButtonName);
        settingsButton = menuPause.Q<Button>(settingsButtonName);
        quitButton = menuPause.Q<Button>(quitButtonName);
    }

    void RegisterButtonCallbacks()
    {
        resumeButton?.RegisterCallback<ClickEvent>((evt) => GameScreenController.SetPause(false));
        settingsButton?.RegisterCallback<ClickEvent>((evt) => SetSettingsScreen(true));
        quitButton?.RegisterCallback<ClickEvent>((evt) =>
        {
            // exit to main menu
        });
    }


    void DisplayVisualElement(VisualElement element, bool state)
    {
        element.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void BlurBackground(bool state)
    {
        if (volume == null)
            return;

        if (volume.profile.TryGet(out DepthOfField blurDOF))
        {
            blurDOF.active = state;
        }
    }

    void SetPauseScreen(bool show)
    {
        DisplayVisualElement(menuPause, show);
        DisplayVisualElement(inGameUI, !show);
        BlurBackground(show);
    }

    public void SetSettingsScreen(bool show)
    {
        DisplayVisualElement(menuPause, !show);
        DisplayVisualElement(menuSettings, show);
        BlurBackground(show);
    }
}
