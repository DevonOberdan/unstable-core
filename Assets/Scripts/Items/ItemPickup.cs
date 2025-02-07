using UnityEngine;
using UnityEngine.Events;

public enum ItemType
{
    GravityBoots, Booster, Ammo, CoreEnergy
}

public class ItemPickup : MonoBehaviour
{
    [SerializeField] ItemType itemType;
    [SerializeField] int count;

    [SerializeField] GameObject visualEffectObject;
    [SerializeField] UnityEvent OnPickup;
    
    public int Count { get => count; set => count = value; }
    public ItemType Type => itemType;

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
        else if (other.CompareTag("Core"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Planet"))
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<PickupEvent>(PickupEffects);
    }
}