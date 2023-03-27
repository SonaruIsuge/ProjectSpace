using System.Collections.Generic;
using UnityEngine;


public class PlayerPairActManager : MonoBehaviour
{
    [field: SerializeField] public List<Transform> PlayerTargetPosList { get; private set; }
    [SerializeField] private List<Transform> PlayerOriginPosList;


    public void ResetPlayerPosition(Player player, int index)
    {
        player.transform.position = PlayerOriginPosList[index].position;
    }


    // public async void MovePlayerIn(Player player, int index)
    // {
    //     
    // }
    //
    //
    // public async void MovePlayerOut(Player player, int index)
    // {
    //     
    // }
}
