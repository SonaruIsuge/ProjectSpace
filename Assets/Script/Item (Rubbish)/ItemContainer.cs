using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Container", menuName = "Item(Rubbish) System/Container")]
public class ItemContainer : ScriptableObject
{
    [SerializeField] private List<GameObject> AllItem;
    private Dictionary<ItemType, GameObject> ItemDict;

    /// <summary>
    /// Need generate dictionary before using other functions
    /// </summary>
    public void GenerateDictionary()
    {
        ItemDict = new Dictionary<ItemType, GameObject>();
        foreach (var obj in AllItem)
        {
            if(!obj.TryGetComponent<Item>(out var item)) continue;
            ItemDict.Add(item.ItemData.type, obj);
        }
    }


    public GameObject GetItemByType(ItemType type) => ItemDict != null && ItemDict.ContainsKey(type) ? ItemDict[type] : null;
}
