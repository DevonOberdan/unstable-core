using UnityEngine;
using UnityEngine.Events;

public class InventoryRestrictionHandler : MonoBehaviour
{
    [SerializeField] private ItemType itemType;

    [SerializeField] private UnityEvent<bool> OnSetRestriction;

    private void OnEnable()
    {
        EventManager.AddListener<InventoryChangedEvent>(DetermineRestriction);

    }

    private void OnDisable()
    {
        EventManager.RemoveListener<InventoryChangedEvent>(DetermineRestriction);
    }


    void DetermineRestriction(InventoryChangedEvent evt)
    {
        if(evt.itemId == (int)itemType)
        {
            OnSetRestriction.Invoke(evt.itemCount == 0);
        }
    }

    public void ItemUsed()
    {
        Inventory.Instance.UseItem(itemType);
    }

}
