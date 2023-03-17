
using System.Collections.Generic;
using Script.Item__Rubbish_;
using SonaruUtilities;
using UnityEngine;

public class DataManager : TSingletonMonoBehaviour<DataManager>
{
    [SerializeField] private ItemContainer itemContainer;
    [SerializeField] private RecycleRule recycleRule;

    private Dictionary<ItemType, GameObject> itemContainerDict;
    private Dictionary<RecycleType, ItemTypeCollection> recycleRuleDict;

    public Material SelectItemMat;

    protected override void Awake()
    {
        base.Awake();
        itemContainerDict = itemContainer.GenerateDictionary();
        recycleRuleDict = recycleRule.GenerateDictionary();
    }


    public GameObject GetItem(ItemType type)
    {
        if (itemContainerDict == null || !itemContainerDict.ContainsKey(type)) return null;
        return itemContainerDict[type];
    }


    public bool CheckRecyclable(ItemType itemType, RecycleType recycleType) =>
        recycleRuleDict[recycleType].ItemTypes.Contains(itemType);
}
