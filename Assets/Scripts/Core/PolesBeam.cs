using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolesBeam : MonoBehaviour
{

    [SerializeField] Core core;
    Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        core.OnColorSet += SetColor;
    }

    private void OnDisable()
    {
        core.OnColorSet -= SetColor;

    }

    void SetColor(Color newColor, float time)
    {
        mat.DOColor(newColor, time);
    }
}
