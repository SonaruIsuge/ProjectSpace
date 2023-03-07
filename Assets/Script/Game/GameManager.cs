
using System;
using SonaruUtilities;
using UnityEngine;


public class GameManager : TSingletonMonoBehaviour<GameManager>
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MachineManager machineManager;
    [SerializeField] private ItemManager itemManager;

    public int RemainRubbish;
    
    protected override void Awake()
    {
        base.Awake();
        
    }


    private void OnEnable()
    {
        
    }


    private void Update()
    {
        
    }
    
    
    
}
