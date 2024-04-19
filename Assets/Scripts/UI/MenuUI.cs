using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuUI : MonoBehaviour
{
    public Action OnOpen, OnClose;

    [SerializeField] protected string uiName;
    [SerializeField] protected string elementPrefix;

    [SerializeField] protected bool canClose;

    // maybe use for when we arent always closing the "PreviousMenu"
    public enum MenuType { OVERLAY, SCREEN }

    MenuUI previousMenu;

    // manager is parent
    protected SceneMenuManager sceneMenuManager;
    protected UIDocument document;

    protected VisualElement root;

    bool subPage;

    #region Properties
    public MenuUI PreviousMenu { get => previousMenu; set => previousMenu = value; }

    public bool CanClose => canClose;
    public bool IsVisible => root.visible;//root != null && root.style.display == DisplayStyle.Flex;
    public string UIName => uiName;

    public VisualElement Root => root;
    public string ElementPrefix => elementPrefix;

    public UIDocument Document { get => document; set => document = value; }

    #endregion

    public void DisplayVisualElement(VisualElement element, bool display)
    {
        if (element != null)
            element.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        element.visible = display;
    }

    public void SetDisplayUI(bool display) => DisplayVisualElement(root, display);

    public virtual void Open()  => SetOpen(OnOpen, true);
    public virtual void Close() => SetOpen(OnClose, false);

    public void SetOpen(bool open)
    {
        if (open)
            Open();
        else
            Close();
    }

    void SetOpen(Action action, bool display)
    {
        action?.Invoke();

        SetDisplayUI(display);
        if (PreviousMenu)
            PreviousMenu.SetDisplayUI(!display);
    }

    protected virtual void Awake()
    {
        GrabDocument();
        GrabVisualElements();
        RegisterCallbacks();
    }

    protected virtual void GrabDocument()
    {
        //sceneMenuManager = FindObjectOfType<SceneMenuManager>();
        //Document = FindObjectOfType<UIDocument>();

        //if (sceneMenuManager != null)
        //    Document = sceneMenuManager.GetComponent<UIDocument>();
        //else
        //    Document = GetComponent<UIDocument>();

        if(!TryGetComponent(out document))
        {
            Document = FindObjectOfType<UIDocument>();
            subPage = true;
        }
    }

    protected virtual void GrabVisualElements()
    {
        if (subPage)
            root = document.rootVisualElement.Q(uiName);
        else
            root = document.rootVisualElement;
    }

    protected virtual void RegisterCallbacks()
    {

    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void OnValidate()
    {
        uiName = this.GetType().Name;
    }

    protected string GetUINameFormat(string name)
    {
        string retVal = "";

        if (ElementPrefix != string.Empty)
            retVal = ElementPrefix;

        string[] splitName = SplitCamelCase(name).ToArray();


        for (int i = 0; i < splitName.Length - 1; i++)
        {
            retVal += splitName[i].ToLower();
            retVal += '-';
        }

        retVal += splitName[^1].ToLower();
        return retVal;
    }

    IEnumerable<string> SplitCamelCase(string source)
    {
        const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
        MatchCollection matches = Regex.Matches(source, pattern);
        foreach (Match match in matches)
            yield return match.Value;
    }
}