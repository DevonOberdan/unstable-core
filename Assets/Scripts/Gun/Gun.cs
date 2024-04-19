using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [SerializeField] UnityEvent OnFired;
    [SerializeField] UnityEvent OnEmpty;

    [SerializeField] List<GunConfig> gunModes;
    [SerializeField] GameObject projectilePrefab;

    [SerializeField] Transform barrelPoint;

    GunConfig data;
    Camera mainCamera;

    CameraShake cameraShake;

    float timeSinceFired;
    bool midFire;

    float timeHeld;
    bool charging, charged, chargeFired;

    BeamEffect newBeam;

    delegate void FireRound();
    FireRound Shoot;

    KeyCode fireButton = KeyCode.Mouse0;

    public GunConfig Data
    {
        get => data;
        set
        {
            if (data == value)
                return;

            data = value;

            if (data.fireType == FireType.SINGLE)
                Shoot = ShootSingle;
            else if (data.fireType == FireType.BURST)
                Shoot = ShootBurst;
            else if (data.fireType == FireType.AUTOMATIC)
                Shoot = ShootAutomatic;
            else if (data.fireType == FireType.BEAM)
                Shoot = ShootBeam;
        }
    }


    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.TryGetComponent(out cameraShake);

       //OnFired.AddListener(() => cameraShake.AddTrauma(data.ShakeAmount));
        Data = gunModes[0];
        fireButton = KeyCode.Mouse0;
    }

    void GetGunMode()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Data = gunModes[0];
            fireButton = KeyCode.Mouse0;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Data = gunModes[1];
            fireButton = KeyCode.Mouse1;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.AcceptingGameInput == false)
            return;

        timeSinceFired += Time.deltaTime;

        GetGunMode();

        if(Data.fireType == FireType.BEAM)
        {
            if (!HoldingFire() && newBeam != null)
            {
                newBeam.EndBeam();
                newBeam = null;
            }
        }



        if (data.ChargeUp)
        {
            if (CanShoot())
            {
                ProcessChargeTime();
            }
            else if(!Inventory.Instance.CanUseItem(ItemType.Ammo))
            {
                charged = false;
                chargeFired = false;
                charging = false;
                timeHeld = 0;
            }
        }
    }

    void ProcessChargeTime()
    {
        if (charging && Input.GetKey(fireButton))//Input.GetMouseButton(0))
            timeHeld += Time.deltaTime;
        else
            timeHeld = 0f;

        if (timeHeld >= data.ChargeTime)
            charged = true;
    }

    void ProcessChargeInput()
    {
        if (Input.GetKeyDown(fireButton))
        {
            charging = true;
            chargeFired = false;
        }
        else if (Input.GetKeyUp(fireButton))
        {
            if (chargeFired)
                charged = false;
            chargeFired = false;
            charging = false;
        }
    }

    bool IsChargeTriggered          => data.InstantRelease   || Input.GetKeyUp(fireButton);
    bool ShouldFireSingleChargeShot => data.ChargeSingleShot && Input.GetKeyUp(fireButton);// && !chargeFired;

    public bool FireInput => Data.CanHoldFire ? Input.GetKey(fireButton) : Input.GetKeyDown(fireButton);

    bool HoldingFire()
    {
        bool chargeBool = Data.ChargeUp ? charged : true;

        return CanShoot() && chargeBool && FireInput;
    }

    void LateUpdate()
    {
        if (GameManager.Instance.AcceptingGameInput == false)
            return;

        if (CanShoot())
        {
            if (data.ChargeUp)
            {
                if (data.CanHoldFire)
                    ProcessHoldCharge();
                else
                    ProcessNonHoldCharge();
            }
            else if (FireInput)
            {
                Shoot();
            }
        }
        else if (FireInput)
        {
            OnEmpty.Invoke();
        }

        // set charge states
        if (data.ChargeUp)
            ProcessChargeInput();
    }

    void ProcessHoldCharge()
    {
        if (charged)
        {
            Shoot();
            chargeFired = true;
            charging = false;
        }
    }

    void ProcessNonHoldCharge()
    {
        if (charged)
        {
            if (IsChargeTriggered)
            {
                Shoot();
                chargeFired = data.InstantRelease;
                charged = false;
                charging = false;
            }
        }
        else if (ShouldFireSingleChargeShot)
        {
            Shoot();
        }
    }

    bool CanShoot() => (!data.RequiresAmmo || Inventory.Instance.CanUseItem(ItemType.Ammo)) &&
                       timeSinceFired > data.TimeBetweenShots &&
                       !midFire && (data.CanHoldFire || !chargeFired);



    #region Shoot Mode Functions
    void ShootSingle()
    {
        //Vector3 cursorPoint = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, mainCamera.nearClipPlane));
        //Ray rayFromCursor = new Ray(cursorPoint, mainCamera.transform.forward);
        //RaycastHit hit;

        Fire(barrelPoint.position, FindShotLine());

        Inventory.Instance.UseItem(ItemType.Ammo);
        OnFired.Invoke();
        timeSinceFired = 0;

        void Fire(Vector3 spawnPos, Vector3 dir) => Instantiate(Data.Projectile, spawnPos, Quaternion.identity)
                                                                .GetComponent<Projectile>().Fire(dir, data.BulletSpeed);

        Vector3 FindShotLine()
        {
            Vector3 cursorPoint = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, mainCamera.nearClipPlane));
            Ray rayFromCursor = new Ray(cursorPoint, mainCamera.transform.forward);

            if (Physics.Raycast(rayFromCursor, out RaycastHit hit, 1000, ~LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore))
                return (hit.point - barrelPoint.position).normalized;
            else
                return barrelPoint.transform.forward;
        }
    }

    void ShootBurst() => StartCoroutine(ProcessBurst());

    void ShootAutomatic()
    {
        if (timeSinceFired > data.TimeBetweenRounds)
            ShootSingle();
    }

    void ShootBeam()
    {
        Vector3 rayCastStart = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, mainCamera.nearClipPlane));

        if (newBeam == null)
        {
            newBeam = Instantiate(Data.Projectile, barrelPoint.transform.parent).GetComponent<BeamEffect>();
        }

        if (timeSinceFired > data.TimeBetweenRounds)
        {

            if (Physics.Raycast(rayCastStart, mainCamera.transform.forward, out RaycastHit hit, 10000, ~(1 << LayerMask.NameToLayer("Player")), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.gameObject.TryGetComponent(out Core core) && core.CurrentCoreState == Core.CoreState.EXTRACTABLE)
                {
                    Debug.DrawLine(rayCastStart, hit.point, Color.cyan);
                    core.Extract(0.001f, hit, mainCamera.transform.forward);
                }
            }
            timeSinceFired = 0;
        }
    }

    IEnumerator ProcessBurst(){
        midFire = true;
        ShootSingle();
        
        int i = 1;
        while(i<data.BurstCount && Inventory.Instance.CanUseItem(ItemType.Ammo))
        {
            yield return new WaitForSeconds(data.TimeBetweenRounds);
            ShootSingle();
            i++;
        }
        midFire = false;
    }
    #endregion
}
