

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
        if (isMove && disFromDestination <= 0.1f)
        {
            RotateForward();
            if (Vector3.Angle(player.forward, targetPosition.forward) < 1)
            {
                isMove = false;    
            }
            
        }
        
        playerAni.SetBool("IsGround", true);
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


    public void Ready()
    {
        playerAni.SetBool("Ready", true);
    }


    private float currentRotate;
    private void RotateForward()
    {
        var rotateY = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetPosition.eulerAngles.y, ref currentRotate, 0.1f);
        
        player.transform.rotation = Quaternion.Euler(0, rotateY, 0);
        //player.Rotate(0, rotateY * Time.deltaTime, 0);
    }
}
