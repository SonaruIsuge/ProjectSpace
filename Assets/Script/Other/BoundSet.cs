
using System.Collections.Generic;
using UnityEngine;

public class BoundSet : MonoBehaviour
{
    [SerializeField] private GameObject forwardBound;
    [SerializeField] private GameObject backBound;
    [SerializeField] private GameObject leftBound;
    [SerializeField] private GameObject rightBound;
    [SerializeField] private GameObject upBound;
    [SerializeField] private GameObject downBound;

    [SerializeField] private float far;
    [SerializeField] private float near;
    private List<Vector3> eightPoint;
    private Camera mainCam => Camera.main;
    
    private void Awake()
    {
        eightPoint = GetEightPoint();

        GenerateBounds(GetEightPoint());
    }


    private List<Vector3> GetEightPoint()
    {
        var camPos = mainCam.transform.position;
        var camForward = mainCam.transform.forward;
        
        var camRight = Vector3.Cross(Vector3.up, camForward).normalized;
        var camUp = Vector3.Cross(camForward, camRight).normalized;
        
        var nearClip = mainCam.nearClipPlane;

        var halfHeight = nearClip * Mathf.Tan(mainCam.fieldOfView * Mathf.Deg2Rad / 2);
        var halfWidth = halfHeight * mainCam.aspect;

        var tlVector = camForward * nearClip + camUp * halfHeight - camRight * halfWidth;
        var blVector = camForward * nearClip - camUp * halfHeight - camRight * halfWidth;
        var trVector = camForward * nearClip + camUp * halfHeight + camRight * halfWidth;
        var brVector = camForward * nearClip - camUp * halfHeight + camRight * halfWidth;

        var farTopLeft = camPos + tlVector * ((far - camPos.z) / tlVector.z);
        var farBottomLeft = camPos + blVector * ((far - camPos.z) / blVector.z);
        var farTopRight = camPos + trVector * ((far - camPos.z) / trVector.z);
        var farRightBottom = camPos + brVector * ((far - camPos.z) / brVector.z);
        var nearTopLeft = camPos + tlVector * ((near - camPos.z) / tlVector.z);
        var nearBottomLeft = camPos + blVector * ((near - camPos.z) / blVector.z);
        var nearTopRight = camPos + trVector * ((near - camPos.z) / trVector.z);
        var nearRightBottom = camPos + brVector * ((near - camPos.z) / brVector.z);

        var result = new List<Vector3>()
        {
            farTopLeft,
            farBottomLeft,
            farTopRight,
            farRightBottom,
            nearTopLeft,
            nearBottomLeft,
            nearTopRight,
            nearRightBottom
        };

        Debug.DrawLine(result[0], result[1], Color.red, 100f);
        Debug.DrawLine(result[2], result[3], Color.red, 100f);
        Debug.DrawLine(result[4], result[5], Color.red, 100f);
        Debug.DrawLine(result[6], result[7], Color.red, 100f);
        Debug.DrawLine(result[0], result[2], Color.red, 100f);
        Debug.DrawLine(result[1], result[3], Color.red, 100f);
        Debug.DrawLine(result[4], result[6], Color.red, 100f);
        Debug.DrawLine(result[5], result[7], Color.red, 100f);
        Debug.DrawLine(result[0], result[4], Color.red, 100f);
        Debug.DrawLine(result[1], result[5], Color.red, 100f);
        Debug.DrawLine(result[2], result[6], Color.red, 100f);
        Debug.DrawLine(result[3], result[7], Color.red, 100f);

        return result;
    }


    private void GenerateBounds(List<Vector3> points)
    {
        CalcBound(forwardBound.transform, points[0], points[1], points[2], points[3]);
        CalcBound(backBound.transform, points[4], points[5], points[6], points[7]);
        CalcBound(leftBound.transform, points[0], points[1], points[4], points[5]);
        CalcBound(rightBound.transform, points[2], points[3], points[6], points[7]);
        CalcBound(upBound.transform, points[0], points[2], points[4], points[6]);
        CalcBound(downBound.transform, points[1], points[3], points[5], points[7]);
    }


    private void CalcBound(Transform trans, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        var minX = Mathf.Min(p1.x, p2.x, p3.x, p4.x);
        var maxX = Mathf.Max(p1.x, p2.x, p3.x, p4.x);
        var minY = Mathf.Min(p1.y, p2.y, p3.y, p4.y);
        var maxY = Mathf.Max(p1.y, p2.y, p3.y, p4.y);
        var minZ = Mathf.Min(p1.z, p2.z, p3.z, p4.z);
        var maxZ = Mathf.Max(p1.z, p2.z, p3.z, p4.z);
        
        var normal = Vector3.Cross(p2 - p1, p3 - p2).normalized;
        var rotation = Quaternion.LookRotation(-normal);

        trans.position = new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
        //trans.localScale = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
        trans.rotation = rotation;
        trans.localScale = new Vector3(80, 80, 1);
    }
    
}
