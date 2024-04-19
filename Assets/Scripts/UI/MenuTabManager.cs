using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuTabManager : MonoBehaviour
{
    [SerializeField] List<string> buttonNames;

    [SerializeField] string tabClass;
    [SerializeField] string toggledClassSuffix;

    MenuUI menuUI;

    List<Button> tabs;
    Dictionary<Button, MenuUI> tabDict;
    Button startTab;

    public List<Button> Tabs => tabs;

    private void Start()
    {
        menuUI = GetComponent<MenuUI>();
        menuUI.OnOpen += ShowStartTab;

        GetTabReferences();
        RegisterTabEvents();
    }


    public void GetTabReferences()
    {
        tabDict = new();
        tabs = new();

        for (int i = 0; i < buttonNames.Count; i++)
        {
            tabs.Add(menuUI.Root.Q<Button>(menuUI.ElementPrefix + buttonNames[i]));
        }

        for (int i = 0; i < tabs.Count; i++)
        {
            tabDict.Add(tabs[i], transform.GetChild(i).GetComponent<MenuUI>());
        }

        startTab = tabs[0];
    }

    public void RegisterTabEvents()
    {
        foreach (Button tab in Tabs)
            tab.clicked += () => SelectTab(tab);
    }

    public void ShowStartTab() => SelectTab(startTab);

    public void SelectTab(Button newActiveTab)
    {
        foreach (Button tab in tabDict.Keys)
        {
            // set tabs
            if (tab == newActiveTab)
                tab.AddToClassList(tabClass + toggledClassSuffix);
            else
                tab.RemoveFromClassList(tabClass + toggledClassSuffix);

            // set pages
            tabDict[tab].SetDisplayUI(tabDict[tab] == tabDict[newActiveTab]);
        }
    }
}
