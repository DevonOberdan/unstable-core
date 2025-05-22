using UnityEngine;
using UnityEngine.Events;
using ScriptableObjectLibrary;

public enum ItemType
{
    GravityBoots, Booster, Ammo, CoreEnergy
}

public class ItemPickup : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    [SerializeField] private ItemTypeSO type;
    [SerializeField] int count;

    [SerializeField] GameObject visualEffectObject;
    [SerializeField] UnityEvent OnPickup;
    
    public int Count { get => count; set => count = value; }
    public ItemTypeSO Type => type;

    void Start()
    {
        EventManager.AddListener<PickupEvent>(PickupEffects);
    }

    void PickupEffects(PickupEvent evt)
    {
        if (evt.item != this)
            return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Events.onPickup.item = this;
            EventManager.Broadcast(Events.onPickup);

            OnPickup.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<PickupEvent>(PickupEffects);
    }
}