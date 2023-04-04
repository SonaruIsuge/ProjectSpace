using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class PlayerPairActManager : MonoBehaviour
{
    [SerializeField] private List<Transform> players;
    [field: SerializeField] public List<Transform> PlayerTargetPosList { get; private set; }
    [SerializeField] private List<Transform> PlayerOriginPosList;


    private List<PairPlayerController> pairPlayerControllers;
    

    /// <summary>
    /// Create player list and reset all players position to origin position.
    /// </summary>
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


    /// <summary>
    /// Move player to target position.
    /// </summary>
    /// <param name="index">which player move in</param>
    public void MovePlayerIn(int index)
    {
        pairPlayerControllers[index].MoveIn();
    }
    
    
    /// <summary>
    /// Move player to origin position.
    /// </summary>
    /// <param name="index">which player move out</param>
    public void MovePlayerOut(int index)
    {
        pairPlayerControllers[index].MoveOut();
    }


    /// <summary>
    /// Make player play ready animation
    /// </summary>
    /// <param name="index">which player play the animation</param>
    public void SetPlayerReadyAni(int index)
    {
        pairPlayerControllers[index].Ready();
    }
}
