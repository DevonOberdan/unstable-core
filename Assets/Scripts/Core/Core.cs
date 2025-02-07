using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Core : MonoBehaviour
{
    public enum CoreState { HEATING, COOLING, EXTRACTABLE }

    [SerializeField] UnityEvent OnShrink;

    [Header("Heating Fields")]
    public AnimationCurve heatCurve;
    [SerializeField] Collider finalSizeReference;

    [Range(0.25f, 3f)]
    [SerializeField] private float heatAccelerationFactor;

    [SerializeField] float minHeat = 1;
    [SerializeField] float maxHeat = 60;
    [SerializeField] float currentHeat;

    [SerializeField] bool heatUp = true;

    [Header("Cooldown")]
    [SerializeField] float coolDownShrinkTime = 1.5f;
    [SerializeField] float revUpTime = 1f;

    [SerializeField] float extractableSpeedFactor = 0.05f;
    [SerializeField] float coolingSpeedFactor = 0.5f;

    [SerializeField] bool stallAfterCooling;

    [DrawIf(nameof(stallAfterCooling), true)]
    [SerializeField] float coolStallTime = 2f;

    [SerializeField] AnimationCurve cooldownCurve;
    [SerializeField] AnimationCurve cooldownRecoveryCurve;

    public UnityAction<float, float> OnRotationSet;
    public UnityAction<Color, float> OnColorSet;

    [Header("Extraction Fields")]
    [SerializeField] bool allowExtraction;

    public enum ExtractionMode { CONSTANT, TIMED }

    [DrawIf(nameof(allowExtraction), true, DrawIfAttribute.DisablingType.ReadOnly)]
    public ExtractionMode extractionMode;

    [DrawIf(nameof(allowExtraction), true, DrawIfAttribute.DisablingType.ReadOnly)]
    [Range(0, 1)]
    [SerializeField] float extractionThreshold;

    [DrawIf(nameof(allowExtraction), true, DrawIfAttribute.DisablingType.ReadOnly)]
    [SerializeField] float extractionTime = 2f;

    [DrawIf(nameof(allowExtraction), true, DrawIfAttribute.DisablingType.ReadOnly)]
    [SerializeField] int stackedHitsToExtract;

    [DrawIf(nameof(allowExtraction), true, DrawIfAttribute.DisablingType.ReadOnly)]
    [SerializeField] bool startExtractable;


    [Header("Visuals")]
    [SerializeField] Vector2 shaderIntensityRange;
    [SerializeField] Vector2 shaderFactorRange;
    [SerializeField] Color extractColor;

    [SerializeField] GameObject coreEnergyPrefab;
    [SerializeField] float energyExtractionForce = 0;

    [SerializeField] float energySpawnTime = 2f;
    [SerializeField] int energySpawnCount = 5;

    CoreState currentCoreState;
    float stabilityFactor = 1f;
    float scaleFactor = 1;
    bool exploded = false;
    float defaultRotationSpeed = 1;

    Tweener coolingTween;
    float timeSinceCooldown = 0;

    new Collider collider;
    Material coreMat;

    Tweener rotTween;
    Tweener sloMoTween;

    Color heatColor;
    Vector3 minSize;
    float curvePosition = 0;
    int stackedHits;
    float energyTimer=0f;

    public bool ShouldExplode => CurrentHeat >= maxHeat && !exploded;

    public float CurrentHeat
    {
        get => currentHeat;
        set
        {
            currentHeat = Mathf.Clamp(value, minHeat, maxHeat);

            MeltCore();

            float intensityDiff = shaderIntensityRange[1] - shaderIntensityRange[0];
            float factorDiff = shaderFactorRange[1] - shaderFactorRange[0];
            coreMat.SetFloat("_Shake_Intensity", shaderIntensityRange[0] + intensityDiff * heatCurve.Evaluate(curvePosition));
            coreMat.SetFloat("_Shake_Factor", shaderFactorRange[0] + factorDiff * heatCurve.Evaluate(curvePosition));
        }
    }

    public CoreState CurrentCoreState
    {
        get => currentCoreState;
        set
        {
            currentCoreState = value;
            stackedHits = 0;

            if (currentCoreState == CoreState.HEATING)
            {
                stackedHits = 0;
                //coreMat.DOColor(heatColor, revUpTime);
                OnColorSet(heatColor, revUpTime);
                heatUp = true;
            }
            else if (currentCoreState == CoreState.EXTRACTABLE)
            {
                OnColorSet(extractColor, coolDownShrinkTime);
                //coreMat.DOColor(extractColor, coolDownShrinkTime);

                if (sloMoTween.IsActive())
                    sloMoTween.Kill();
                sloMoTween = rotTween.DOTimeScale(defaultRotationSpeed * extractableSpeedFactor, coolDownShrinkTime).SetEase(Ease.Linear);
                OnRotationSet(defaultRotationSpeed * extractableSpeedFactor, coolDownShrinkTime);

                Invoke(nameof(EndExtraction), extractionTime);
            }
        }
    }

    public float Stability
    {
        get => stabilityFactor;
        set => stabilityFactor = Mathf.Clamp01(value);
    }

    bool ExtractableThresholdMet => curvePosition < extractionThreshold && stackedHits >= stackedHitsToExtract;

    void Start()
    {
        minSize = new Vector3(0.5f,0.5f,0.5f);
        collider = GetComponent<Collider>();
        
        coreMat = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
        heatColor = coreMat.color;

        float minBounds = collider.bounds.size.magnitude;
        float maxBounds = finalSizeReference.bounds.size.magnitude;
        scaleFactor = (maxBounds / minBounds)-1f;

        Vector3 rot = new Vector3(360, 360, 360);
        rotTween = transform.DORotate(rot, 10f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);

        CurrentHeat = Mathf.Max(minHeat, CurrentHeat);

        if (startExtractable)
        {
            CurrentHeat = maxHeat / 2;
            extractionTime = 1000f;
            CurrentCoreState = CoreState.EXTRACTABLE;
        }
    }

    void Update()
    {
        if (ShouldExplode)
            Explode();

        if (CurrentCoreState == CoreState.COOLING)
        {
            HandleCooldown();
        }
        else if(CurrentCoreState == CoreState.HEATING && heatUp)
        {
            timeSinceCooldown = Mathf.Clamp(timeSinceCooldown + Time.deltaTime, 0, revUpTime);
            CurrentHeat += ((timeSinceCooldown / revUpTime) * Time.deltaTime) * (2/Stability) * heatAccelerationFactor;
        }
        else if(CurrentCoreState == CoreState.EXTRACTABLE)
        {
            //
        }

        energyTimer += Time.deltaTime;
    }

    private void OnEnable()
    {
        OnColorSet += (color, time) => coreMat.DOColor(color, time);
    }

    private void OnDisable()
    {
        OnColorSet -= (color, time) => coreMat.DOColor(color, time);
    }

    void Explode()
    {
        exploded = true;
        
        if(rotTween != null)
            rotTween.timeScale = 0;
        
        GameManager.Instance.CoreFullEnd();
        this.enabled = false;
    }

    void HandleCooldown()
    {
        timeSinceCooldown = 0;
    }

    void MeltCore()
    {
        curvePosition = CurrentHeat / maxHeat;
        transform.localScale = heatCurve.Evaluate(curvePosition) * scaleFactor * minSize;
    }

    public void Cooldown(float cooldownFactor)
    {
        if (CurrentCoreState == CoreState.EXTRACTABLE)
            return;

        OnShrink.Invoke();

        stackedHits++;

        if (coolingTween.IsActive())
            coolingTween.Kill();

        currentCoreState = CoreState.COOLING;

        float reducedPosition = curvePosition - (curvePosition * cooldownFactor * Stability);

        coolingTween = DOTween.To(() => CurrentHeat, x => CurrentHeat = x, reducedPosition * maxHeat, coolDownShrinkTime)
               .SetEase(cooldownCurve)
               .OnComplete(() => 
                {
                    if (stallAfterCooling)
                        Invoke(nameof(EndCoolStall), coolStallTime * Stability);
                    else
                        EndCoolStall();
                });

        if (sloMoTween.IsActive())
            sloMoTween.Kill();
        sloMoTween = rotTween.DOTimeScale(defaultRotationSpeed * coolingSpeedFactor, coolDownShrinkTime).SetEase(Ease.Linear);
    }

    public float Extract(float extractionAmount, RaycastHit impactData, Vector3 hitDir)
    {
        Stability -= extractionAmount;


        if(energyTimer > energySpawnTime)
        {
            SpawnEnergy(energySpawnCount);
            energyTimer = 0;
        }

        // spawn item

        // set its gravity source to player
        //energyItem.GetComponent<GravityObject>().SetSource()
        // launch it off in the direction of the reflection


        //Inventory.Instance.AddItem(ItemType.CoreEnergy, 1);
        //Inventory.Instance.CoreEnergy += extractionAmount;

        // event that sends extractionAmount over to Inventory (have inventory value be a multiple of this small extractionAmount
        return 1;

        void SpawnEnergy(int count)
        {
            for(int i = 0; i < count; i++)
            {
                GameObject energyItem = Instantiate(coreEnergyPrefab, impactData.point, Quaternion.identity);
                Rigidbody energyItemRB = energyItem.GetComponent<Rigidbody>();
                //energyItemRB.AddForce(Vector3.Reflect(hitDir, impactData.normal) * energyExtractionForce);
                energyItemRB.AddForce(Vector3.ProjectOnPlane(Random.insideUnitSphere, impactData.normal).normalized * energyExtractionForce);
            }
        }
    }

    void EndExtraction()
    {
        timeSinceCooldown = 0;
        sloMoTween = rotTween.DOTimeScale(defaultRotationSpeed, revUpTime);
        OnRotationSet(defaultRotationSpeed, revUpTime);
        CurrentCoreState = CoreState.HEATING;
    }

    void EndCoolStall()
    {
        if (CurrentCoreState == CoreState.EXTRACTABLE)
            return;

        sloMoTween = rotTween.DOTimeScale(1, revUpTime);

        CurrentCoreState = allowExtraction && ExtractableThresholdMet ? CoreState.EXTRACTABLE : CoreState.HEATING;
    }

    private void OnDestroy()
    {
        sloMoTween.Kill();
        rotTween.Kill();
    }
}
