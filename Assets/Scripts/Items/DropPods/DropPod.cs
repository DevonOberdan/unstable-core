using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DropPod : MonoBehaviour
{
    [SerializeField] PodItemMode itemMode;

    [SerializeField] PodDestroyMode destroyMode;

    [SerializeField] List<ItemPickup> itemsToSpawn;

    [SerializeField] Transform itemSpawnPointContainer;


    [DrawIf(nameof(destroyMode), PodDestroyMode.TIMER)]
    [SerializeField] float destroyTime = 2f;

    //[SerializeField] int numberOfPickups = 4;

    [SerializeField] UnityEvent OnPodLanded;
    [SerializeField] UnityEvent OnInitialization;
    [SerializeField] UnityEvent OnItemsSpawned;

    List<ItemPickup> itemPickups;

    [SerializeField] Transform doors;

    DropPodManager manager;
    GravityObject gravityData;
    bool landed, itemPickedUp;

    const float SPAWN_DELAY = 0.5f;
    const float DOOR_DROP = -1.6f;

    void Awake()
    {
        gravityData = GetComponent<GravityObject>();
        itemPickups = new();

        doors.localPosition = Vector3.zero;
        OnPodLanded.AddListener(() => StartCoroutine(BeginSpawn()));
    }

    public void Init(DropPodManager podManager, PodConfiguration config)
    {
        manager = podManager;
        itemMode = config.mode;

        itemsToSpawn.Clear();

        itemsToSpawn.Capacity = itemSpawnPointContainer.childCount;

        // Set itemsToSpawn list based on provided config mode and item type

        if (itemMode == PodItemMode.RANDOM)
        {
            for (int i = 0; i < itemsToSpawn.Capacity; i++)
                itemsToSpawn.Add(manager.AllItems.RandomItem());
        }
        else if (itemMode == PodItemMode.ALL)
        {
            itemsToSpawn = manager.AllItems;
        }
        else if (itemMode == PodItemMode.ALL_OF_ONE)
        {
            for (int i = 0; i < itemsToSpawn.Capacity; i++)
                itemsToSpawn.Add(manager.AllItems.Where(item => item.Type == config.itemType).FirstOrDefault());
        }
    }


    void Update()
    {
        if(!landed && GetComponent<Rigidbody>().constraints == RigidbodyConstraints.FreezeAll)
        {
            landed = true;
            OnPodLanded.Invoke();
        }
    }


    // do any delay/ effects before actually spawning the items
    IEnumerator BeginSpawn()
    {
        yield return new WaitForSeconds(SPAWN_DELAY*4);

        OnInitialization.Invoke();
        
        SpawnItems();

        doors.DOLocalMoveY(DOOR_DROP, SPAWN_DELAY);
    }

    void SpawnItems()
    {
        OnItemsSpawned.Invoke();


        for (int i = 0; i < itemsToSpawn.Count; i++)
        {
            Transform spawnPoint = itemSpawnPointContainer.GetChild(i);
            ItemPickup spawnedItem = Instantiate(itemsToSpawn[i], spawnPoint.position, Quaternion.identity, spawnPoint);

            if(spawnedItem.TryGetComponent(out RotateObject rotateObject))
            {
                rotateObject.RotationSpeed = Random.Range(5, 13);

                int randomDir = Random.Range(0, 2);
                rotateObject.SetDirection(randomDir == 0);
            }

            EventManager.AddListener<PickupEvent>(HandlePickupEvent);


            spawnedItem.GetComponent<GravityObject>().GravityFlipped = gravityData.GravityFlipped;
            itemPickups.Add(spawnedItem);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        bool shot = collision.collider.gameObject.layer == LayerMask.NameToLayer("Projectile");
        if (destroyMode == PodDestroyMode.SHOOT && itemPickedUp && shot)
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Core"))
            DestroyPod();

        if (collision.gameObject.CompareTag("Planet"))
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            gravityData.enabled = false;
        }
    }


    void DestroyTimedOnPickup(PickupEvent evt)
    {
        if (!itemPickups.Contains(evt.item))
            return;
        DestroyPod();
    }

    void HandlePickupEvent(PickupEvent evt)
    {
        if (!itemPickups.Contains(evt.item))
            return;

        itemPickedUp = true;

        itemPickups.Remove(evt.item);

        Destroy(evt.item);

        if(itemPickups.Count == 0)
        {
            if (destroyMode == PodDestroyMode.ONPICKUP)
                Destroy(gameObject);
            else if (destroyMode == PodDestroyMode.TIMER)
                DestroyPod();
        }
    }

    void DestroyPod()
    {

        // blow up or something
        StartCoroutine(DelayDestruction());
        //Destroy(gameObject);
        //maybe just pool drop pods instead of destroying?
    }

    IEnumerator DelayDestruction()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<PickupEvent>(HandlePickupEvent);
    }
}