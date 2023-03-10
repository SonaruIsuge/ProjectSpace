using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera[] allVirtualCameras;
    private CinemachineVirtualCamera currentActiveCam;
    private float offsetDegree;
    private Camera mainCam => Camera.main;
    
    [field: SerializeField] public float currentRotate { get; private set; }
    private bool enableAutoRotate;
    
    private void Awake()
    {
        allVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        
        foreach(var cam in allVirtualCameras)
            if (cam.gameObject.activeInHierarchy) currentActiveCam = cam;
        
        var pos = currentActiveCam.transform.position;
        offsetDegree = Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;
        currentRotate = offsetDegree;

        enableAutoRotate = false;
    }
    

    private void Update()
    {
        if(enableAutoRotate) RotateCamAroundYAxis(5 * Time.deltaTime);
    }


    public void EnableAuto() => enableAutoRotate = !enableAutoRotate;
    
    
    public void RotateCamAroundYAxis(float rotateDeg)
    {
        currentActiveCam.transform.RotateAround(Vector3.zero, Vector3.up, rotateDeg);
        currentRotate += rotateDeg;
        if (currentRotate >= 360) currentRotate -= 360;
        if (currentRotate < 0) currentRotate += 360;
    }
}
