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

    float timeSinceFlip;

    GravityObject playerGravity;
    FPSMovement playerMovement;

    readonly string gravityFlipper = "GravityFlipper";
    float currentChargeTime;
    bool isFlipping;

    Vector3 preFlipRotation, currentRotation;

    Tween playerFlipTween;

    int currentFlippedDir;

    public float CurrentBoost { get; set; }
    public float FullChargeTime => fullChargeTime;
    public float CurrentChargeTime => currentChargeTime;

    public bool EnteringPlanet => playerGravity.GravityFlipped && currentDistance < FlipDistance && !isFlipping;
    public bool ExitingPlanet => !playerGravity.GravityFlipped && currentDistance > FlipDistance && !isFlipping;

    public float FlipDistance { get; private set; }
    float currentDistance;
    private Vector3 rotationDirection;

    private void Awake()
    {
        playerGravity = GetComponentInParent<GravityObject>();
        playerMovement = GetComponentInParent<FPSMovement>();

        Vector3 planetExtents = playerGravity.Source.GetComponent<MeshRenderer>().bounds.extents;
        float avgRadius = (planetExtents.x + planetExtents.y + planetExtents.z) / 3;
        FlipDistance = avgRadius;
    }

    private void FixedUpdate()
    {
        if (bootMode == BootMode.DRAIN && playerGravity.GravityFlipped)
            HandleBootDrain();

        currentDistance = Vector3.Distance(transform.position, playerGravity.Source.transform.position);
        HandleDistance();
    }

    void HandleDistance()
    {
        if (playerMovement.IsInAir && EnteringPlanet)
            SetGravity(false);
        else if (playerMovement.IsInAir && ExitingPlanet)
            SetGravity(true);
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

    Vector3 gravityDirection;
    Vector3 toCenterGravityDirection;
    Vector3 awayFromCenterGravityDirection;

    void CheckFlip(Collider other, bool flip)
    {
        print("Test");
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

    void EndFlip() => isFlipping = false;

    IEnumerator Accelerate(bool flip)
    {
        playerGravity.GravityFactor = thresholdAccelerationVal;
        //playerGravity.GravityDir = -currentFlippedDir;
        // playerGravity.GravityFlipped = !playerGravity.GravityFlipped;

        yield return new WaitForSeconds(.2f);

        playerGravity.GravityFactor = 1;
        //playerGravity.GravityDir = currentFlippedDir;

        playerGravity.GravityFlipped = flip;
    }
}