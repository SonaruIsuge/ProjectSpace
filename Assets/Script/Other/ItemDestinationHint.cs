using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX.Utility;


public class ItemDestinationHint : MonoBehaviour
{
    [SerializeField] private CurveGroup curveParticle;
    
    [SerializeField] private int curveStep;
    [SerializeField] private float curveSpeed;
    [SerializeField] private int initialNum;
    
    private Queue<CurveGroup> particlePool;


    private void Awake()
    {
        particlePool = new Queue<CurveGroup>();
        for (var i = 0; i < initialNum; i++ ) InitParticle();
    }   


    private void InitParticle()
    {
        var particle = Instantiate(curveParticle);
        particle.pool = particlePool;
        particle.ReturnPool();
    }


    public async Task SpawnCurve(Vector3 originPos, Vector3 targetPos)
    {
        if (particlePool.Count == 0) InitParticle();
        var group = particlePool.Dequeue();
        group.gameObject.SetActive(true);
        group.Init(originPos);

        var routes = group.GetCurve(originPos, targetPos, curveStep);

        var lastPoint = originPos;
        while (routes.Count > 0)
        {
            var targetPoint = routes.Dequeue();
            
            Debug.DrawLine(lastPoint, targetPoint, Color.red, 100f);
            lastPoint = targetPoint;
            
            while (Vector3.Distance(group.target.position, targetPoint) > 0.01f)
            {
                group.target.position = Vector3.MoveTowards(group.target.position, targetPoint, curveSpeed * Time.deltaTime);
                await Task.Yield();
            }
        }

        await Task.Delay(1500);
        group.ReturnPool();
    }
}