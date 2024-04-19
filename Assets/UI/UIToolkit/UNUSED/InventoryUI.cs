using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [SerializeField] GravityBoots boots;
    [SerializeField] string inGameUIName = "InventoryUI";

    [SerializeField] UIDocument gameScreenDoc;

    UIDocument uiDoc;
    VisualElement root;

    List<Label> uiCounts;

    ProgressBar boostBar;

    Label bootDrainLabel;

    string LABEL_SUFFIX = "Count";

    private void OnDestroy()
    {
        Instance = null;
    }

    void Awake()
    {
        Instance = this;

        //uiDoc = GetComponent<UIDocument>();
        uiDoc = gameScreenDoc;

        //root = uiDoc.rootVisualElement;
        root = uiDoc.rootVisualElement.Q(inGameUIName);
        print(root.name);
        //boostBar = root.Q<ProgressBar>("BoostBar");
        // boostBar.value = boostBar.highValue;

        // bootDrainLabel = root.Q<Label>("GravityBootsChargeLevel");
        // bootDrainLabel.visible = false;

        //DOTween.Twe
        print(Inventory.Instance.name);

        //Inventory.Instance.OnInventoryChanged += SetItem;
        //Inventory.Instance.OnInventoryChanged += (index, count) => UIEffect(index);

        SetupItemCounts();
    }

    void SetupItemCounts()
    {
        uiCounts = new List<Label>();

        foreach (var itemName in Enum.GetNames(typeof(ItemType)))
        {
            Label label = root.Q<Label>(itemName + LABEL_SUFFIX);
            if (label != null)
                uiCounts.Add(label);
        }
    }

    public void SetItem(int itemIndex, int count) => uiCounts[itemIndex].text = "" + count;

    /// <summary>
    /// run particular visual effects when you use the item, I'm sure this can be better than checking each index
    /// </summary>
    /// <param name="itemIndex"></param>
    public void UIEffect(int itemIndex)
    {
        if(itemIndex == 0)
        {
            //
        }
        else if (itemIndex == 1)
        {

        }
    }


    void Update()
    {


        if(boots.bootMode == GravityBoots.BootMode.DRAIN)
        {
            //bootDrainLabel.visible = boots.GravityFlipped;
            bootDrainLabel.text = (boots.CurrentChargeTime / boots.FullChargeTime).ToString("0.00");
        }
    }

    public void ShowBootText(bool show) => bootDrainLabel.visible = show;

    public void SetBootText(string text) => bootDrainLabel.text = text;

}
