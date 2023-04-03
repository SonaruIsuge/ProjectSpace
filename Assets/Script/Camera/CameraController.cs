using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    private List<CinemachineVirtualCamera> allRotatableCam;

    [SerializeField] private CinemachineVirtualCamera originVirtualCam;

    [SerializeField] private float ellipseMajorAxis;
    private CinemachineVirtualCamera currentActiveCam;
    private Camera mainCam => Camera.main;

    [SerializeField] private float rotateUnitAngle;

    private float currentRotate;
    public float CurrentRotate
    {
        get => currentRotate;
        private set
        {
            currentRotate = value;
            if (currentRotate >= 360) currentRotate -= 360;
            if (currentRotate < 0) currentRotate += 360;
        }
    }
    
    private int currentRotateIndex;
    private bool isRotating;
    
    private bool enableAutoRotate;


    private void OnDrawGizmos()
    {
        var currentPos = originVirtualCam.transform.position;
        var center = new Vector3(0, currentPos.y, 0);
        
        Gizmos.color = Color.magenta;
        
        for (var i = rotateUnitAngle; i < 360f; i += rotateUnitAngle)
        {
            var nextPos = CircleRotateAroundAxis(currentPos, Vector3.up, rotateUnitAngle);
            //RotatePointOnEllipse(center, ellipseMajorAxis, Vector3.Distance(originVirtualCam.transform.position, center), i);
            Gizmos.DrawLine(currentPos, nextPos);
            currentPos = nextPos;
        }
        Gizmos.DrawLine(currentPos, originVirtualCam.transform.position);
    }


    private void Awake()
    {
        allRotatableCam = new List<CinemachineVirtualCamera>();
        
        currentActiveCam = originVirtualCam;
        
        var pos = currentActiveCam.transform.position;
        CurrentRotate = Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;

        isRotating = false;
        enableAutoRotate = false;

        allRotatableCam.Add(currentActiveCam);
        for (var i = rotateUnitAngle; i < 360f; i += rotateUnitAngle)
        {
            var cam = Instantiate(currentActiveCam, transform);
            cam.transform.RotateAround(Vector3.zero, Vector3.up, i);
            allRotatableCam.Add(cam);
            cam.gameObject.SetActive(false);
        }

        currentRotateIndex = 0;
    }
    

    private void Update()
    {
        if(enableAutoRotate) RotateCamAroundYAxis(5 * Time.deltaTime);
    }


    public void EnableAuto() => enableAutoRotate = !enableAutoRotate;
    
    
    public void RotateCamAroundYAxis(float rotateDeg)
    {
        currentActiveCam.transform.RotateAround(Vector3.zero, Vector3.up, rotateDeg);
        CurrentRotate += rotateDeg;
    }


    public async void RotateCam(Player player, float clockwise)
    {
        if(clockwise == 0) return;
        if(isRotating) return;
        
        isRotating = true;
        var rotateDir = clockwise > 0 ? 1 : -1;
        
        foreach(var cam in allRotatableCam) cam.gameObject.SetActive(false);

        currentRotateIndex += rotateDir;
        if (currentRotateIndex < 0) currentRotateIndex = allRotatableCam.Count - 1;
        if (currentRotateIndex >= allRotatableCam.Count) currentRotateIndex = 0;
        currentActiveCam = allRotatableCam[currentRotateIndex];
        currentActiveCam.gameObject.SetActive(true);

        CurrentRotate += rotateUnitAngle * rotateDir;
        
        
        await Task.Delay(500);
        isRotating = false;
    }


    private Vector3 CircleRotateAroundAxis(Vector3 originPos, Vector3 axis, float angle)
    { 
        return Quaternion.AngleAxis(angle, axis) * originPos;
    }


    // public static Vector3 RotatePointOnEllipse(Vector3 centerPos, float majorAxis, float minorAxis, float angle)
    // {
    //     var x = centerPos.x + majorAxis * Mathf.Cos(angle);
    //     var y = centerPos.y;
    //     var z = centerPos.z + minorAxis * Mathf.Sin(angle);
    //     return new Vector3(x, y, z);
    // }
    //
    //
    // private Vector3 RotateAroundYAxis(Vector3 originPos, Vector3 center, float majorAxis, float angle)
    // {
    //     var minerAxis = Vector3.Distance(originPos, center);
    //     var majorPoint = Vector3.Cross(originPos, Vector3.up).normalized * majorAxis;
    //     
    // }
}
