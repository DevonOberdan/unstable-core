using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GridBrushBase;

public class GravityBoots : MonoBehaviour
{
    public enum BootMode { SINGLE_USE, DRAIN }
    public BootMode bootMode;

    [SerializeField] UnityEvent OnBoost;

    [SerializeField] float thresholdAccelerationVal = 2;

    [DrawIf(nameof(bootMode), BootMode.DRAIN)]
    [SerializeField] float fullChargeTime = 10;

    [SerializeField] float flipSpeed = 4f;
    [SerializeField] float flipTime = 1f;
    [SerializeField] float flipScalar = 4f;

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

    public bool EnteringPlanet => playerGravity.GravityFlipped && currentDistance < FlipDistance;
    public bool ExitingPlanet => !playerGravity.GravityFlipped && currentDistance > FlipDistance;

    public float FlipDistance { get; private set; }
    float currentDistance;
    private Vector3 rotationDirection;

    private void Awake()
    {
        //GetCom
        playerGravity = GetComponentInParent<GravityObject>();
        playerMovement = GetComponentInParent<FPSMovement>();

        playerGravity.OverrideDirection = true;


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



    private void Update()
    {
        PlayerGravityHelper();

        //if(timeSinceFlip < flipTime)
        //{
        //   // playerGravity.transform.rotation = Quaternion.Slerp(playerGravity.transform.rotation, playerGravity.CurrentGravityRotation, (timeSinceFlip/flipTime));
        //    timeSinceFlip += Time.deltaTime;
        //}
        //else
        //{
        //    timeSinceFlip = 0;
        //    playerGravity.OverrideDirection = false;
        //}




        //if (isFlipping)
        //{
        //    Vector3 endDirection = playerGravity.CurrentGravityRotation.eulerAngles;

        //    rotationDirection = Vector3.Cross(playerGravity.transform.right, endDirection).normalized;

        //    float angle = Vector3.Angle(currentRotation, endDirection);
        //    print(angle);
        //    if (angle >= 3.0f)
        //    {
        //        currentRotation = Vector3.RotateTowards(currentRotation, endDirection + rotationDirection, flipSpeed * Time.deltaTime, 0.0f);
        //        playerGravity.CurrentGravityDir = currentRotation;
        //    }
        //    else
        //    {
        //        isFlipping = false;

        //        playerGravity.SetGrabSourceDir(true);
        //    }
        //}
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

        preFlipRotation = playerGravity.CurrentGravityRotation.eulerAngles;

        playerGravity.GravityFlipped = flip;
        currentFlippedDir = playerGravity.GravityDir;

        StartCoroutine(Accelerate());
        OnBoost.Invoke();

        if (bootMode == BootMode.SINGLE_USE)
            Inventory.Instance.UseItem(ItemType.GravityBoots);

        currentRotation = preFlipRotation;



        //playerGravity.SetStanding(false);
        timeSinceFlip = 0;
        playerGravity.OverrideDirection = true;
        isFlipping = true;
        //playerFlipTween = playerGravity.transform.DORotateQuaternion(playerGravity.CurrentGravityRotation, flipTime)
        //                           .OnComplete(() => playerGravity.SetStanding(true));

        //Invoke(nameof(EndFlip), 1f);
    }

    Vector3 gravityDirection;
    Vector3 toCenterGravityDirection;
    Vector3 awayFromCenterGravityDirection;
    private void PlayerGravityHelper()
    {
        //calculate gravity rotation directions continuously
        //awayFromCenterGravityDirection = -toCenterGravityDirection;

        toCenterGravityDirection = Vector3.zero - gameObject.transform.position;
        awayFromCenterGravityDirection = -toCenterGravityDirection;

        //when gravity is not flipping, set gravity direction based on which side of sphere player is on
        //otherwise if gravity is flipping, calculate rotation direction and rotate gravity direction vector towards the appropriate direction
        //once gravity direction has reached its goal, gravity is no longer flipping and normal calculations resume

        // toCenterGravityDirection = Vector3.zero - gameObject.transform.position;
        //awayFromCenterGravityDirection = -toCenterGravityDirection;

        //when gravity is not flipping, set gravity direction based on which side of sphere player is on
        //otherwise if gravity is flipping, calculate rotation direction and rotate gravity direction vector towards the appropriate direction
        //once gravity direction has reached its goal, gravity is no longer flipping and normal calculations resume
        if (!isFlipping)
        {
            if (playerGravity.GravityFlipped) playerGravity.GravityDirection = toCenterGravityDirection;
            else playerGravity.GravityDirection = awayFromCenterGravityDirection;

            //playerGravity.GravityDirection = playerGravity.FinalGravityDirection;
        }
        else
        {
            rotationDirection = Vector3.Cross(transform.forward, playerGravity.GravityDirection).normalized;

            if (playerGravity.GravityFlipped)
            {
                if (Vector3.Angle(playerGravity.GravityDirection, toCenterGravityDirection) >= 3.0f)
                {
                    playerGravity.GravityDirection = Vector3.RotateTowards(playerGravity.GravityDirection, 
                                                                           toCenterGravityDirection + rotationDirection, 
                                                                           flipScalar * Time.deltaTime, 0.0f);
                }
                else
                {
                    isFlipping = false;
                }
            }
            else if (!playerGravity.GravityFlipped)
            {
                if (Vector3.Angle(playerGravity.GravityDirection, awayFromCenterGravityDirection) >= 3.0f)
                {
                    playerGravity.GravityDirection = Vector3.RotateTowards(playerGravity.GravityDirection, 
                                                                           awayFromCenterGravityDirection + rotationDirection, 
                                                                           flipScalar * Time.deltaTime, 0.0f);
                }
                else
                {
                    isFlipping = false;
                }
            }
        }



        //if (!isFlipping)
        //{
        //    gravityDirection = -playerGravity.FinalGravityDirection;

        //    //if (isGravityTowardsCenter) gravityDirection = toCenterGravityDirection;
        //    //else gravityDirection = awayFromCenterGravityDirection;
        //}
        //else
        //{
        //    rotationDirection = Vector3.Cross(transform.forward, gravityDirection).normalized;

        //    if (Vector3.Angle(gravityDirection, playerGravity.FinalGravityDirection) >= 3.0f)
        //    {
        //       // Debug.Log($"Gravity set by: <color=blue>Boots</color>");
        //        playerGravity.GravityDirection = Vector3.RotateTowards(gravityDirection, playerGravity.FinalGravityDirection + rotationDirection, flipScalar * Time.deltaTime, 0.0f);
        //    }
        //    else
        //    {
        //        Debug.Log("<color=green>Gravity Boots turned off.</color>");

        //        isFlipping = false;
        //       // playerGravity.OverrideDirection = false;
        //    }
        //}
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

   // void EndFlip() => isFlipping = false;

    IEnumerator Accelerate()
    {
        playerGravity.GravityFactor = thresholdAccelerationVal;
        //playerGravity.GravityDir = -currentFlippedDir;
       // playerGravity.GravityFlipped = !playerGravity.GravityFlipped;

        yield return new WaitForSeconds(.2f);

        playerGravity.GravityFactor = 1;
       // playerGravity.GravityDir = currentFlippedDir;
       // playerGravity.GravityFlipped = !playerGravity.GravityFlipped;

    }
}