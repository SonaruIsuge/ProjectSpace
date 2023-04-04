
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera originVirtualCam;
    [SerializeField] private List<CinemachineVirtualCamera> allRotatableCam;

    private Vector3 RotateCenter => new (0, originVirtualCam.m_Priority, 0);
   
    private CinemachineVirtualCamera currentActiveCam;
    private int currentRotateIndex;
    
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
    
    private bool isRotating;
    [SerializeField] private bool showGizmos;

    private void OnDrawGizmos()
    {
        if(!showGizmos) return;
        if(allRotatableCam == null || allRotatableCam.Count == 0) return;

        Gizmos.color = Color.yellow;
        for (var i = 0; i < allRotatableCam.Count - 1; i++)
        {
            Gizmos.DrawLine(allRotatableCam[i].transform.position, allRotatableCam[i+1].transform.position);
        }
        Gizmos.DrawLine(allRotatableCam.First().transform.position, allRotatableCam.Last().transform.position);
    }
    

    private void Awake()
    {
        foreach(var cam in allRotatableCam) cam.gameObject.SetActive(false);
        originVirtualCam.gameObject.SetActive(true);

        currentActiveCam = originVirtualCam;
        currentRotateIndex = 0;
        isRotating = false;

        CurrentRotate = CalcCamRotate();
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

        CurrentRotate = CalcCamRotate();
        
        await Task.Delay(500);
        isRotating = false;
    }


    private float CalcCamRotate()
    {
        var originPos = RotateCenter - originVirtualCam.transform.position;
        var currentPos = RotateCenter - currentActiveCam.transform.position;
        var angle = Vector3.Angle(originPos, currentPos);
        var sign = Mathf.Sign(Vector3.Dot(Vector3.Cross(originPos, currentPos), originVirtualCam.transform.up));
        return angle * sign;
    }


    public void EditorSwitchCamera(int direction)
    {
        if(!originVirtualCam) return;
        if(allRotatableCam.Count == 0) return;

        direction = direction > 0 ? 1 : -1;
        
        foreach(var vc in allRotatableCam) vc.gameObject.SetActive(false);

        currentRotateIndex += direction;
        if (currentRotateIndex < 0) currentRotateIndex = allRotatableCam.Count - 1;
        if (currentRotateIndex >= allRotatableCam.Count) currentRotateIndex = 0;
        
        allRotatableCam[currentRotateIndex].gameObject.SetActive(true);
    }
}
