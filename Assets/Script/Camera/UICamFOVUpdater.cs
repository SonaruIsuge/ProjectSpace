using System;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class UICamFOVUpdater : MonoBehaviour
{
    [SerializeField] private Camera targetCam;
    private Camera uICam;


    private void Awake()
    {
        uICam = GetComponent<Camera>();
    }


    private void Update()
    {
        uICam.fieldOfView = targetCam.fieldOfView;
    }
}
