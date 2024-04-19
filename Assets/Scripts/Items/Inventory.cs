using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 
/// 
/// 8/20/2022
/// </summary>
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    Dictionary<int, int> itemCounts;

    //public static Action<int,int> OnInventoryChanged;

    //public float coreEnergy = 0;

    //public float CoreEnergy {
    //    get => coreEnergy;
    //    set {
    //        coreEnergy = value;

    //    }
    //}
    [SerializeField] bool debugEquipAtStart;
    [SerializeField] List<GunConfig> guns;

    public bool IsEmpty()
    {
        foreach(int key in itemCounts.Keys)
        {
            if (itemCounts[key] > 0)
                return false;
        }
        return true;
    }

    public int ItemCount(ItemType type)
    {
        if(!itemCounts.ContainsKey((int)type))
            return 0;

        return itemCounts[(int)type];
    }

    public bool AnyItemsRequired(List<ItemType> types)
    {
        foreach (ItemType key in types)
        {
            if (itemCounts[(int)key] == 0)
                return true;
        }
        return false;
    }

    public ItemType ItemMostNeeded() 
    {
        return (ItemType) itemCounts.Values.OrderBy(value => value).First();
    }

    public bool CanUseItem(ItemType type) => itemCounts[(int)type] > 0;
    public void UseItem(ItemType type)
    {
        itemCounts[(int)type]--;
        InventoryChanged((int)type);
        //OnInventoryChanged((int)type, itemCounts[(int)type]);
    }

    public void AddItem(ItemType type, int count)
    {
        itemCounts[(int)type] += count;
        InventoryChanged((int)type);
        //OnInventoryChanged((int)type, itemCounts[(int)type]);
    }

    void InventoryChanged(int type)
    {
        InventoryChangedEvent inventoryChanged = Events.onInventoryChanged;
        inventoryChanged.itemId = type;
        inventoryChanged.itemCount = itemCounts[(int)type];
        EventManager.Broadcast(inventoryChanged);
    }

    private void Awake()
    {
        Instance = this;
        EventManager.AddListener<PickupEvent>(evt => AddItem(evt.item.Type, evt.item.Count));

        itemCounts = new();
        int[] typeIds = (int[])Enum.GetValues(typeof(ItemType));

        foreach (int id in typeIds)
        {
            itemCounts.Add(id, debugEquipAtStart ? 1000 : 0);
            //OnInventoryChanged(id, 0);
            InventoryChanged(id);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
        EventManager.RemoveListener<PickupEvent>(evt => AddItem(evt.item.Type, evt.item.Count));
    }
}