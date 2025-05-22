using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [SerializeField] bool debugEquipAtStart;
    [SerializeField] private List<ItemTypeSO> items;

    Dictionary<ItemTypeSO, int> itemCounts;

    public List<ItemTypeSO> Items => items;

    private void Awake()
    {
        Instance = this;
        EventManager.AddListener<PickupEvent>(evt => AddItem(evt.item.Type, evt.item.Count));

        itemCounts = new();
    }

    private void Start()
    {
        foreach (ItemTypeSO data in items)
        {
            itemCounts.Add(data, debugEquipAtStart ? 1000 : 0);
            InventoryChanged(data);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        EventManager.RemoveListener<PickupEvent>(evt => AddItem(evt.item.Type, evt.item.Count));
    }

    public bool IsEmpty()
    {
        foreach(ItemTypeSO key in itemCounts.Keys)
        {
            if (itemCounts[key] > 0)
                return false;
        }
        return true;
    }

    public int ItemCount(ItemTypeSO type)
    {
        if(!itemCounts.ContainsKey(type))
            return 0;

        return itemCounts[type];
    }

    public bool AnyItemsRequired(List<ItemTypeSO> types)
    {
        foreach (ItemTypeSO key in types)
        {
            if (itemCounts[key] == 0)
                return true;
        }
        return false;
    }

    public ItemTypeSO ItemMostNeeded() 
    {
        return itemCounts.OrderBy(keyValue => keyValue.Value).First().Key;
    }

    public bool CanUseItem(ItemTypeSO type)
    {
        return itemCounts[type] > 0;
    }

    public void UseItem(ItemTypeSO type)
    {
        itemCounts[type]--;
        InventoryChanged(type);
    }

    public void AddItem(ItemTypeSO type, int count)
    {
        itemCounts[type] += count;
        InventoryChanged(type);
    }

    void InventoryChanged(ItemTypeSO type)
    {
        InventoryChangedEvent inventoryChanged = Events.onInventoryChanged;
        inventoryChanged.itemId = type;
        inventoryChanged.itemCount = itemCounts[type];
        EventManager.Broadcast(inventoryChanged);
    }
}