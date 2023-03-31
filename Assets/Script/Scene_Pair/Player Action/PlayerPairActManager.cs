using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerPairActManager : MonoBehaviour
{
    [SerializeField] private List<Transform> players;
    [field: SerializeField] public List<Transform> PlayerTargetPosList { get; private set; }
    [SerializeField] private List<Transform> PlayerOriginPosList;


    private List<PairPlayerController> pairPlayerControllers;
    

    public void ResetPlayersPosition()
    {
        pairPlayerControllers = new List<PairPlayerController>();
        for (var i = 0; i < players.Count; i++)
        {
            players[i].transform.position = PlayerOriginPosList[i].position;
            players[i].transform.LookAt(PlayerTargetPosList[i]);
            
            pairPlayerControllers.Add(new PairPlayerController(players[i], PlayerOriginPosList[i], PlayerTargetPosList[i]));
        }
    }


    public void UpdatePlayerAct()
    {
        foreach (var playerCon in pairPlayerControllers)
        {
            playerCon.UpdateController();
        }
    }


    public void MovePlayerIn(int index)
    {
        pairPlayerControllers[index].MoveIn();
    }
    
    
    public void MovePlayerOut(int index)
    {
        pairPlayerControllers[index].MoveOut();
    }


    public void SetPlayerReadyAni(int index)
    {
        pairPlayerControllers[index].Ready();
    }
}
