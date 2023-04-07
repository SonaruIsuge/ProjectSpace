using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CurveGroup : MonoBehaviour
{
    public Transform target;
    public Transform body;
    public Queue<CurveGroup> pool;

    private Queue<Vector3> curvePoints;

    public void Init(Vector3 pos)
    {
        target.position = pos;
        //body.position = pos;
    }


    public void ReturnPool()
    {
        pool.Enqueue(this);
        gameObject.SetActive(false);
        
    }


    public Queue<Vector3> GetCurve(Vector3 startPoint, Vector3 endPoint, int step)
    {
        var result = new Queue<Vector3>();
        var distance = Vector3.Distance(startPoint, endPoint);
        var referPoint = (startPoint + endPoint) / 2 + Vector3.up * distance;

        for (var i = 0; i < step; i++)
        {
            var t = i / (float)step;
            var u = 1 - t;
            var point = u * u * startPoint + t * u * referPoint + t * t * endPoint;
            
            result.Enqueue(point);
        }

        return result;
    }
}