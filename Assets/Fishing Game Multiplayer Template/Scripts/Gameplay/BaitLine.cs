using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitLine : MonoBehaviour
{
    [Header("Bait Line")]
    [SerializeField] public Transform _FloatEndPoint;
    [SerializeField] public Transform _BaitPoint;
    [SerializeField] public LineRenderer _rodLineRenderer;

    private void Update()
    {
        _rodLineRenderer.SetPosition(0, _FloatEndPoint.position);
        _rodLineRenderer.SetPosition(1, _BaitPoint.transform.position);
    }
}
