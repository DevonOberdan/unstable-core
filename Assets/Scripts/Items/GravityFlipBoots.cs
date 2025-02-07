using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GravityFlipBoots : MonoBehaviour
{
    public enum BootMode { SINGLE_USE, DRAIN }
    public BootMode bootMode;

    [SerializeField] UnityEvent OnBoost;
    [SerializeField] float thresholdAccelerationVal = 2;

    [DrawIf(nameof(bootMode), BootMode.DRAIN)]
    [SerializeField] float fullChargeTime = 10;

    public float CurrentBoost { get; set; }
    public float FullChargeTime => fullChargeTime;
    public float CurrentChargeTime => currentChargeTime;

    public bool EnteringPlanet => playerGravity.GravityFlipped && currentDistance < FlipDistance && !isFlipping;
    public bool ExitingPlanet => !playerGravity.GravityFlipped && currentDistance > FlipDistance && !isFlipping;
    public float FlipDistance { get; private set; }

    GravityObject playerGravity;
    FPSMovement playerMovement;

    Vector3 preFlipRotation, currentRotation;
    Vector3 rotationDirection, gravityDirection;
    Vector3 toCenterGravityDirection, awayFromCenterGravityDirection;

    bool isFlipping;
    int currentFlippedDir;

    float currentChargeTime;
    float currentDistance;

    readonly string gravityFlipper = "GravityFlipper";

    void Awake()
    {
        playerGravity = GetComponentInParent<GravityObject>();
        playerMovement = GetComponentInParent<FPSMovement>();

        Vector3 planetExtents = playerGravity.Source.GetComponent<MeshRenderer>().bounds.extents;
        float avgRadius = (planetExtents.x + planetExtents.y + planetExtents.z) / 3;
        FlipDistance = avgRadius;
    }

    void FixedUpdate()
    {
        if (bootMode == BootMode.DRAIN && playerGravity.GravityFlipped)
            HandleBootDrain();

        currentDistance = Vector3.Distance(transform.position, playerGravity.Source.transform.position);
        HandleDistance();
    }

    public void SetGravity(bool flip)
    {
        if (!Inventory.Instance.CanUseItem(ItemType.GravityBoots))
            return;

        currentFlippedDir = playerGravity.GravityDir;

        StartCoroutine(Accelerate(flip));
        OnBoost.Invoke();

        if (bootMode == BootMode.SINGLE_USE)
            Inventory.Instance.UseItem(ItemType.GravityBoots);

        isFlipping = true;

        Invoke(nameof(EndFlip), 1f);
    }

    void HandleDistance()
    {
        if (playerMovement.IsInAir && EnteringPlanet)
            SetGravity(false);
        else if (playerMovement.IsInAir && ExitingPlanet)
            SetGravity(true);
    }

    void CheckFlip(Collider other, bool flip)
    {
        if (CompareTag("Player") && other.gameObject.CompareTag(gravityFlipper) && !isFlipping)
        {
            if (Inventory.Instance.CanUseItem(ItemType.GravityBoots))
                SetGravity(flip);
        }
    }

    void HandleBootDrain()
    {
        currentChargeTime = Mathf.Clamp(CurrentChargeTime - Time.fixedDeltaTime, 0, FullChargeTime);

        if (CurrentChargeTime == 0)
        {
            if (Inventory.Instance.CanUseItem(ItemType.GravityBoots))
            {
                Inventory.Instance.UseItem(ItemType.GravityBoots);
                currentChargeTime = FullChargeTime;
            }
            else
            {
                SetGravity(false);
            }
        }
    }

    void EndFlip()
    {
        isFlipping = false;
    }

    IEnumerator Accelerate(bool flip)
    {
        playerGravity.GravityFactor = thresholdAccelerationVal;

        yield return new WaitForSeconds(.2f);

        playerGravity.GravityFactor = 1;
        playerGravity.GravityFlipped = flip;
    }
}