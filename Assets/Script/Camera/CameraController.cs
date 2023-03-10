using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private List<CinemachineVirtualCamera> allVirtualCameras;
    private CinemachineVirtualCamera currentActiveCam;
    private List<CinemachineVirtualCamera> allRotatableCam;
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
    
    private void Awake()
    {
        allVirtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>().ToList();
        allRotatableCam = new List<CinemachineVirtualCamera>();
        
        foreach (var cam in allVirtualCameras.Where(cam => cam.gameObject.activeInHierarchy)) currentActiveCam = cam;
        
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
            allVirtualCameras.Add(cam);
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


    public async void RotateCam(float clockwise)
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
}
