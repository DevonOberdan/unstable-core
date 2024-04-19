using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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

    Tween effectRotation;

    void Start()
    {
        EventManager.AddListener<PickupEvent>(PickupEffects);

       // effectRotation = visualEffectObject.transform.DOLocalRotate(new Vector3(UnityEngine.Random.Range(0,360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)), 5, RotateMode.FastBeyond360)
        //                                             .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).SetRelative(true);
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
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnDestroy()
    {
        //effectRotation.Kill();
        EventManager.RemoveListener<PickupEvent>(PickupEffects);
    }
}
