using UnityEngine;
using DG.Tweening;

public class RotateObject : MonoBehaviour
{
    [SerializeField] Vector3 rotationVector;
    [SerializeField] float rotationSpeed = 1;

    Tweener sloMoTween;
    float dampenFactor = 1f;

    public float DampenFactor
    {
        get => dampenFactor;
        set => dampenFactor = value;
    }

    public float RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }

    void Update()
    {
        transform.Rotate(rotationVector * rotationSpeed * dampenFactor * Time.deltaTime);
    }

    public void SetDirection(bool positive)
    {
        rotationVector *= positive ? 1 : -1;
    }

    public void SetDampenFactor(float newDampen, float shiftTime)
    {
        if (sloMoTween.IsActive())
            sloMoTween.Kill();

        sloMoTween = DOTween.To(() => dampenFactor, x => dampenFactor = x, newDampen, shiftTime);
    }
}