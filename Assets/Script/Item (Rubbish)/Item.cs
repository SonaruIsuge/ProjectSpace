using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Item : MonoBehaviour, IInteractable
{
    public ItemData ItemData;
    [SerializeField] private List<Player> carryPlayers;
    public InteractType InteractType => InteractType.Tap;
    
    public bool isInteract { get; private set; }
    public bool isSelect { get; private set; }

    public Rigidbody Rb { get; private set; }
    private Collider col;

    public event Action<Item, Player> OnNewPlayerInteract;
    public event Action<Item, Player> OnRemovePlayerInteract;
    

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }


    public void OnSelect()
    {
        isSelect = true;
    }
    

    public void Interact(Player interactPlayer, InteractType interactType)
    {
        if (interactType != InteractType) return;
        
        // picked up by player
        if(!carryPlayers.Contains(interactPlayer)) PickUp(interactPlayer);
        else DropDown(interactPlayer);
    }
    

    public void OnDeselect()
    {
        isSelect = false;
    }


    public void PickUp(Player picker)
    {
        if(carryPlayers.Contains(picker)) return;
        
        carryPlayers.Add(picker);
        isInteract = true;
        picker.PlayerInteractController.SetCurrentInteract(this);
        picker.SwitchToRigidbodyMove(true);
        
        picker.Rb.Sleep();
        //Rb.velocity = Vector3.zero;
        //Rb.angularVelocity = Vector3.zero;
        picker.Joint.connectedBody = Rb;
        Rb.Sleep();
        
        OnNewPlayerInteract?.Invoke(this, picker);

    }


    public void DropDown(Player dropper)
    {
        if(!carryPlayers.Contains(dropper)) return;

        carryPlayers.Remove(dropper);
        if(carryPlayers.Count == 0) isInteract = false;
        dropper.PlayerInteractController.SetCurrentInteract(null);
        
        Rb.WakeUp();
        dropper.Joint.connectedBody = null;
        dropper.Rb.WakeUp();
        dropper.SwitchToRigidbodyMove(false);
        
        OnRemovePlayerInteract?.Invoke(this, dropper);
    }


    public Vector3 GetItemCollisionSize()
    {
        return col == null ? Vector3.one : col.bounds.size;
    }
    
}
