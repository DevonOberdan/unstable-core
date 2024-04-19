using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PodDestroyMode
{
    ONPICKUP,
    TIMER,
    SHOOT,
    KEEP
}
public enum PodItemMode { RANDOM, ALL, ALL_OF_ONE, WEIGHTED }


public class DropPodManager : MonoBehaviour
{
    [SerializeField] DropPod dropPodPrefab;

    [SerializeField] BeamEffect incomingSpawnEffectPrefab;

    [SerializeField] int dropRatePerMinute = 10;

    [Header("Scene References")]
    [SerializeField] Player player;

    [Header("Item References")]
    [SerializeField] List<ItemPickup> allItems;


    List<DropPodSpawnPoint> spawnPoints;

    bool active, podHasDropped;

    float timeSinceDrop = 0;
    
    public static Vector3 PodOrientation { get; private set; }

    public List<DropPodSpawnPoint> PointsFromPlayer => spawnPoints.OrderBy(point => Vector3.Distance(point.transform.position, player.transform.position)).ToList();
    public DropPodSpawnPoint ClosestSpawnPoint => PointsFromPlayer.FirstOrDefault();

    public List<ItemPickup> AllItems => allItems;

    float MinimumDropTime => (60 / dropRatePerMinute);

    public bool Active 
    {
        get => active;
        set 
        {
            if(value == active)
                return;

            active = value;
        }
    }


    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<DropPodSpawnPoint>().ToList();

        PodOrientation = new Vector3(90, 0, 0);
    }

    public void DropFirstPod()
    {
        if (!podHasDropped)
        {
            DropClosest();
            podHasDropped = true;
        }
    }

    public void DropClosest() => SpawnAtPoint(ClosestSpawnPoint);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Active = !Active;


        if (!Active)
            return;

        timeSinceDrop += Time.deltaTime;

        if(timeSinceDrop > MinimumDropTime)
        {
            DropPodSpawnPoint selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            if (!selectedPoint.HasDropPod)
            {
                SpawnAtPoint(selectedPoint);
            }
            else
            {
                Debug.Log("Pod already spawned at point");
            }
        }
    }

    public void SpawnAtPoint(DropPodSpawnPoint spawnPoint) => StartCoroutine(SetupDropPodSpawn(spawnPoint));

    IEnumerator SetupDropPodSpawn(DropPodSpawnPoint spawnPoint, bool spawnMultiple = false)
    {
        Transform spawnPos = spawnPoint.GetNewDropPoint();

        spawnPoint.PodSpawnEffect(incomingSpawnEffectPrefab);

        SpawnDropPod(spawnPoint);

        yield return new WaitForSeconds(2.5f);

        spawnPoint.EndEffect();
    }

    public bool SpawnDropPod(DropPodSpawnPoint spawnPoint, bool spawnMultiple = false)
    {
        if (spawnPoint.HasDropPod && !spawnMultiple)
            return false;

        Transform spawnPos = spawnPoint.DropPoint;

        DropPod pod = Instantiate(dropPodPrefab, spawnPoint.StartingSpawnPoint, Quaternion.identity, spawnPoint.transform);

        // initialize pod with the provided configuration
        pod.Init(this, DeterminedPodMode());

        pod.transform.rotation = Quaternion.LookRotation(-spawnPos.forward);

        if(pod.TryGetComponent(out GravityObject podGravity))
            podGravity.SetSource(spawnPos.GetComponent<GravitySource>());

        spawnPoint.CurrentDropPod = pod;

        timeSinceDrop = 0;
        return true;
    }

    /// <summary>
    /// Wrapper that will send back a configuration for what the DropPod should contain.
    /// 
    /// Change the logic here to balance/change the distribution/occurence of different configurations
    /// </summary>
    /// <returns></returns>
    PodConfiguration DeterminedPodMode()
    {
        PodConfiguration config = new PodConfiguration(PodItemMode.RANDOM);


        if (Inventory.Instance.IsEmpty())
        {
            config.mode = PodItemMode.ALL;
        }
        else if (Inventory.Instance.AnyItemsRequired(AllItems.Select(item => item.Type).ToList()))
        {
            config.mode = PodItemMode.ALL_OF_ONE;
            config.itemType = Inventory.Instance.ItemMostNeeded();
        }

        print($"Mode: {config.mode} Type: {config.itemType}");

        return config;
    }

}

public struct PodConfiguration
{
    public PodItemMode mode;
    public ItemType itemType;

    public PodConfiguration(PodItemMode itemMode, ItemType type = ItemType.Ammo)
    {
        this.mode = itemMode;
        itemType = type;
    }
}