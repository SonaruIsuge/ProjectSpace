using Script.Other;
using UnityEngine;


public class RbOnly : MonoBehaviour
{
    [SerializeField] private Player player;


    private void Start()
    {
        player.PlayerMovement = new RbOnlyMove(player);
        player.PlayerGravityController = new RbOnlyGravity(player);
        //player.SetActive(true);
    }
}
