using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class GeometryVCSpawner : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera originVirtualCam;
    [Range(1, 360)] [SerializeField] private float rotateUnitAngle;
    [SerializeField] private bool showGizmos;
    
    private List<CinemachineVirtualCamera> allSpawnedVC;
    private int activeCamIndex;

    private void OnDrawGizmos()
    {
        if(!showGizmos) return;
        if(!originVirtualCam) return;
        if(rotateUnitAngle <= 0) return;
        
        Gizmos.color = Color.magenta;
        
        var currentPos = originVirtualCam.transform.position;

        for (var i = rotateUnitAngle; i <= 360f; i += rotateUnitAngle)
        {
            var nextPos = CircleRotateAroundAxis(currentPos, Vector3.up, rotateUnitAngle);
            Gizmos.DrawLine(currentPos, nextPos);
            currentPos = nextPos;
        }
    }
    
    
    private Vector3 CircleRotateAroundAxis(Vector3 originPos, Vector3 axis, float angle)
    { 
        return Quaternion.AngleAxis(angle, axis) * originPos;
    }


    public void SpawnVC()
    {
        if(!originVirtualCam) return;
        if(allSpawnedVC != null && allSpawnedVC.Count > 0) return;
        
        allSpawnedVC = new List<CinemachineVirtualCamera>();
        
        for (var i = rotateUnitAngle; i < 360f; i += rotateUnitAngle)
        {
            var cam = Instantiate(originVirtualCam, transform);
            cam.transform.RotateAround(Vector3.zero, Vector3.up, i);
            cam.gameObject.SetActive(false);
            allSpawnedVC.Add(cam);
        }

        activeCamIndex = -1;
    }


    public void ClearVC()
    {
        if(allSpawnedVC == null) return;
        if (allSpawnedVC.Count == 0) return;
        
        for (var i = allSpawnedVC.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(allSpawnedVC[i].gameObject);
        }
        
        allSpawnedVC.Clear();
    }
}
