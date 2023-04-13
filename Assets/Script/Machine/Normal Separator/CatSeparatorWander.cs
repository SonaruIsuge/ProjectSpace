using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


[RequireComponent(typeof(NavMeshAgent))]
public class CatSeparatorWander : MonoBehaviour
{
    [SerializeField] private List<Vector3> destinationList;
    [SerializeField] private Transform moveVfx;
    [SerializeField] private float stopTime;

    [SerializeField] private bool debugPoint;
    private NavMeshAgent navMeshAgent;
    private NavMeshPath navMeshPath;
    private bool stopping;
    
    
    private void OnDrawGizmos()
    {
        if(!debugPoint) return;
        if(destinationList.Count == 0) return;
        
        Gizmos.color = Color.green;
        foreach (var point in destinationList)
        {
            Gizmos.DrawLine(transform.position, point);
        }
    }


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshPath = new NavMeshPath();
    }


    public void Init()
    {
        var dest = GetRandomDestination();
        navMeshAgent.SetDestination(dest);
    }
    

    private void Update()
    {

        if (Vector3.Distance(transform.position, navMeshAgent.destination) <= 0.1f)
        {
            FindNewDestination();
        }
    }


    private async void FindNewDestination()
    {
        if(stopping) return;
        stopping = true;

        await Task.Delay((int)(stopTime * 1000));
        
        var dest = GetRandomDestination();
        navMeshAgent.SetDestination(dest);
        moveVfx.gameObject.SetActive(true);
        stopping = false;
    }


    private Vector3 GetRandomDestination()
    {
        var randomIndex = Random.Range(0, destinationList.Count - 1);
        return destinationList[randomIndex];
    }


    private bool CalculateNewPath(Vector3 dest)
    {
        NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, navMeshPath);
        return navMeshPath.status == NavMeshPathStatus.PathComplete;
    }
}
