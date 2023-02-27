using System;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager : MonoBehaviour
{
    [SerializeField] private List<Item> allItem;
    
    public static event Action<Item, Player> OnItemStartInteract;
    public static event Action<Item, Player> OnItemEndInteract;


    private void OnEnable()
    {
        foreach (var item in allItem)
        {
            SetItem(item, false);
            item.OnNewPlayerInteract += ItemStartInteract;
            item.OnRemovePlayerInteract += ItemEndInteract;
        }
    }


    private void ItemStartInteract(Item item, Player picker)
    {
        SetItem(item, true);
        OnItemStartInteract?.Invoke(item, picker);
    }


    private void ItemEndInteract(Item item, Player dropper)
    {
        SetItem(item, false);
        OnItemEndInteract?.Invoke(item, dropper);
    }


    private void SetItem(Item item, bool isInteract)
    {
        //item.Rb.isKinematic = !isInteract;
    }
}
