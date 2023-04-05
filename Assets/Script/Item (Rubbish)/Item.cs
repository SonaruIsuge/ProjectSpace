using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour, IInteractable, IGravityAffectable
{
    public ItemData ItemData;
    [field: SerializeField] public List<Player> CarryPlayers { get; private set; }
    public InteractType InteractType => InteractType.Tap;
    
    public bool isInteract { get; private set; }
    public bool isSelect { get; private set; }
    public bool UnderGravity { get; set; }
    public bool IgnoreGravity { get; private set; }
    public bool CanInteract { get; private set; }

    public Rigidbody Rb { get; private set; }
    private Collider col;
    private MeshRenderer meshRenderer;
    private Color defaultColor;
    private IItemInitSetting itemInitSetting;

    public event Action<Item, Player> OnNewPlayerInteract;
    public event Action<Item, Player> OnRemovePlayerInteract;
    public event Action<Item> OnItemRemove;
    public event Action<Item> OnItemAppear;
    

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        defaultColor = meshRenderer.material.GetColor("_Outline_Color");
        itemInitSetting = GetComponent<IItemInitSetting>();
        
        CanInteract = true;
    }


    public void OnSelect()
    {
        isSelect = true;

        meshRenderer.material.SetColor("_Outline_Color", Color.red);
    }

    public void SetInteractable(bool interactable) => CanInteract = interactable;
    

    public virtual void Interact(Player interactPlayer, InteractType interactType)
    {
        if (interactType != InteractType || !CanInteract) return;
        
        // picked up by player
        if(!CarryPlayers.Contains(interactPlayer)) PickUp(interactPlayer);
        else DropDown(interactPlayer);
    }
    

    public void OnDeselect()
    {
        isSelect = false;
        
        meshRenderer.material.SetColor("_Outline_Color", defaultColor);
    }


    private void PickUp(Player picker)
    {
        if(CarryPlayers.Contains(picker)) return;
        
        CarryPlayers.Add(picker);
        isInteract = true;
        picker.PlayerInteractController.SetCurrentInteract(this);
        picker.SwitchToRigidbodyMove(true);
        
        //picker.Rb.Sleep();
        picker.Joint.connectedBody = Rb;
        //Rb.Sleep();
        
        OnNewPlayerInteract?.Invoke(this, picker);

    }


    private void DropDown(Player dropper)
    {
        if(!CarryPlayers.Contains(dropper)) return;

        CarryPlayers.Remove(dropper);
        if(CarryPlayers.Count == 0) isInteract = false;
        dropper.PlayerInteractController.SetCurrentInteract(null);
        
        //Rb.WakeUp();
        dropper.Joint.connectedBody = null;
        //dropper.Rb.WakeUp();
        dropper.SwitchToRigidbodyMove(false);
        
        OnRemovePlayerInteract?.Invoke(this, dropper);
    }


    public void RemoveItem(bool disappear = true)
    {
        OnItemRemove?.Invoke(this);
        if(disappear) gameObject.SetActive(false);
    }


    public void AddItem()
    {
        gameObject.SetActive(true);
        itemInitSetting?.InitSetting();
        OnItemAppear?.Invoke(this);
    }


    public Vector3 GetItemCollisionSize()=> col == null ? Vector3.one : col.bounds.size;
    
    
    public void ApplyGravity(float gravity)
    {
        IgnoreGravity = CarryPlayers.Count != 0;
        if (CarryPlayers.Count > 0)
        {
            foreach (var player in CarryPlayers.Where(player => player.IgnoreGravity)) IgnoreGravity = true;
        }

        UpdateGravity();
    }
    
    
    private void UpdateGravity() => Rb.useGravity = UnderGravity && !IgnoreGravity;


    public void ForceDisconnect()
    {
        for (var i = CarryPlayers.Count -1; i >= 0; i--)
        {
            DropDown(CarryPlayers[i]);
        }
    }
    

    private void OnCollisionEnter(Collision other)
    {
        if(!other.transform.CompareTag("Blocking")) return;
        
        var contacts = new ContactPoint[10];
        other.GetContacts(contacts);
        var allNormal = contacts.Aggregate(Vector3.zero, (current, contact) => current + contact.normal);
        
        Rb.velocity = Vector3.Reflect(Rb.velocity.normalized, allNormal.normalized) * 2f;
    }
}
