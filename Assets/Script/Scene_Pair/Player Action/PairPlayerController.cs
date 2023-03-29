

using UnityEngine;
using UnityEngine.AI;

public class PairPlayerController
{
    private Transform player;
    private Transform originPosition;
    private Transform targetPosition;

    private NavMeshAgent playerAI;
    private Animator playerAni;
    
    private bool isMove;
    private float disFromDestination => (player.position - playerAI.destination).magnitude;


    public PairPlayerController(Transform bindPlayer, Transform originPos, Transform targetPos)
    {
        player = bindPlayer;
        originPosition = originPos;
        targetPosition = targetPos;

        isMove = false;

        playerAI = player.GetComponent<NavMeshAgent>();
        playerAni = player.GetComponentInChildren<Animator>();
    }


    public void UpdateController()
    {
        if (isMove && disFromDestination <= 0.01f)
        {
            isMove = false;
        }
        
        playerAni.SetBool("Move", isMove);
        playerAni.SetFloat("MoveSpeed",  6);
    }


    public void MoveIn()
    {
        isMove = true;
        playerAI.SetDestination(targetPosition.position);
       
    }


    public void MoveOut()
    {
        isMove = true;
        playerAI.SetDestination(originPosition.position);
    }
}
