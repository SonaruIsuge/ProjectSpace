using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


public class Item : MonoBehaviour, IInteractable, IGravityAffectable
{
    public ItemData ItemData;
    [SerializeField] private List<Player> carryPlayers;
    public InteractType InteractType => InteractType.Tap;
    
    public bool isInteract { get; private set; }
    public bool isSelect { get; private set; }
    public bool UnderGravity { get; set; }
    public bool IgnoreGravity { get; private set; }
    private bool canInteract;

    public Rigidbody Rb { get; private set; }
    private Collider col;
    
    
    public event Action<Item, Player> OnNewPlayerInteract;
    public event Action<Item, Player> OnRemovePlayerInteract;
    public event Action<Item> OnItemRemove;
    public event Action<Item> OnItemAppear;
    

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        canInteract = true;
    }


    public void OnSelect()
    {
        isSelect = true;
    }

    public void SetInteractable(bool interactable) => canInteract = interactable;
    

    public virtual void Interact(Player interactPlayer, InteractType interactType)
    {
        if (interactType != InteractType || !canInteract) return;
        
        // picked up by player
        if(!carryPlayers.Contains(interactPlayer)) PickUp(interactPlayer);
        else DropDown(interactPlayer);
    }
    

    public void OnDeselect()
    {
        isSelect = false;
    }


    private void PickUp(Player picker)
    {
        if(carryPlayers.Contains(picker)) return;
        
        carryPlayers.Add(picker);
        isInteract = true;
        picker.PlayerInteractController.SetCurrentInteract(this);
        picker.SwitchToRigidbodyMove(true);
        
        picker.Rb.Sleep();
        picker.Joint.connectedBody = Rb;
        Rb.Sleep();
        
        OnNewPlayerInteract?.Invoke(this, picker);

    }


    private void DropDown(Player dropper)
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


    public void RemoveItem()
    {
        OnItemRemove?.Invoke(this);
        gameObject.SetActive(false);
    }


    public void AddItem()
    {
        gameObject.SetActive(true);
        OnItemAppear?.Invoke(this);
        
    }


    public Vector3 GetItemCollisionSize()=> col == null ? Vector3.one : col.bounds.size;
    
    
    public void ApplyGravity(float gravity)
    {
        IgnoreGravity = carryPlayers.Count != 0;
        if (carryPlayers.Count > 0)
        {
            foreach (var player in carryPlayers.Where(player => player.IgnoreGravity)) IgnoreGravity = true;
        }

        UpdateGravity();
    }
    
    
    private void UpdateGravity() => Rb.useGravity = UnderGravity && !IgnoreGravity;


    public void ForceDisconnect()
    {
        for (var i = carryPlayers.Count -1; i >= 0; i--)
        {
            DropDown(carryPlayers[i]);
        }
    }
}
