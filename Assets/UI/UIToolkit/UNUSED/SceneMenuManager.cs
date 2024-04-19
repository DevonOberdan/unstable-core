using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SceneMenuManager : MonoBehaviour
{
    [SerializeField] List<MenuUI> menuUIModalList;

    [SerializeField] MenuUI startModal;

    public void DisplayMenu(MenuUI menuToShow)
    {
        // if tab page, turn off EVERYTHING IN LIST, then turn this on

        // if standard new-panel-type UI

        // currently setting current UI to invisible, so just gets replaced
        // need an option to have it just be an overlay that gets stacked on top
        // to ^^, no special logic needed I think, just ensure the visuals of each XML darken/blur/etc
        // the background, then ensure that what you display will show in the full size it should/
        // on top of everything else

        menuToShow.PreviousMenu.SetDisplayUI(false);
        menuToShow.SetDisplayUI(true);
    }

    private void Awake()
    {
      //  foreach (MenuUI child in transform.GetComponentsInChildren<MenuUI>())
      //      child.Document = GetComponent<UIDocument>();
    }


    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out MenuUI child))
                menuUIModalList.Add(child);
        }

        if (startModal == null)
            startModal = menuUIModalList.First(modal => modal.IsVisible);
    }

    public void HideMenus()
    {
        menuUIModalList.ForEach(menu => menu.SetDisplayUI(false));
    }

    public void ShowMenu(MenuUI menuToShow, bool hideAllOthers = true)
    {
        foreach(MenuUI child in menuUIModalList)
        {
            if (hideAllOthers)
                child.SetDisplayUI(child == menuToShow);
            else if (child == menuToShow)
                child.SetDisplayUI(true);
        }
    }

    /// <summary>
    /// use a stack here to programatically go back to whichever menu got
    /// us to the one we are now leaving, even nested menus.
    /// </summary>
    //public void CloseMenu(MenuUI menuToClose)
    //{
    //    menuToClose.SetDisplayUI(false);

    //    if(menuToClose.PreviousMenu != null)
    //        menuToClose.PreviousMenu.SetDisplayUI(true);
    //}
}
