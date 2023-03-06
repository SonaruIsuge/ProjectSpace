using System;
using System.Collections.Generic;
using System.Linq;
using SonaruUtilities;
using UnityEngine;


public class ItemManager : TSingletonMonoBehaviour<ItemManager>
{
    [SerializeField] private ItemContainer itemContainer;
    [SerializeField] private List<Item> allItem;

    [SerializeField] private NormalSeparatorMachine normalSeparatorMachine;
    
    public static event Action<Item, Player> OnItemStartInteract;
    public static event Action<Item, Player> OnItemEndInteract;




    protected override void Awake()
    {
        base.Awake();
        allItem = FindObjectsOfType<Item>().ToList();
        itemContainer.GenerateDictionary();
    }


    private void OnEnable()
    {
        foreach (var item in allItem)
        {
            item.OnNewPlayerInteract += ItemStartInteract;
            item.OnRemovePlayerInteract += ItemEndInteract;
            item.OnItemRemove += ItemSeparate;
            item.OnItemAppear += ItemOutput;
        }

        //normalSeparatorMachine.OnItemSeparated += ItemSeparate;
        normalSeparatorMachine.OnNewItemOutput += ItemOutput;
    }


    private void ItemStartInteract(Item item, Player picker)
    {
        
        OnItemStartInteract?.Invoke(item, picker);
    }


    private void ItemEndInteract(Item item, Player dropper)
    {
        
        OnItemEndInteract?.Invoke(item, dropper);
    }


    private void ItemSeparate(Item inputItem)
    {
        if(!allItem.Contains(inputItem)) return;
        allItem.Remove(inputItem);
    }
    
    
    private void ItemOutput(Item outputItem)
    {
        allItem.Add(outputItem);
        outputItem.OnNewPlayerInteract += ItemStartInteract;
        outputItem.OnRemovePlayerInteract += ItemEndInteract;
        outputItem.OnItemRemove += ItemSeparate;
        outputItem.OnItemAppear += ItemOutput;
    }


    public GameObject GetItem(ItemType type) => itemContainer.GetItemByType(type);
}
