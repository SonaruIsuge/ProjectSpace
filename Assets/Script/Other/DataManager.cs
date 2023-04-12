
using System.Collections.Generic;
using System.Linq;
using Script.Item__Rubbish_;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class DataManager : TSingletonMonoBehaviour<DataManager>
{
    [SerializeField] private ItemProductsContainer productsContainer;
    [SerializeField] private RecycleRule recycleRule;
    [SerializeField] private RecycleHintContainer recycleHint;

    private Dictionary<ItemType, GameObject> separateProductDict;
    private Dictionary<RecycleType, ItemTypeCollection> recycleRuleDict;
    private Dictionary<RecycleType, Sprite> recycleHintDict;

    public Material SelectItemMat;

    protected override void Awake()
    {
        base.Awake();
        separateProductDict = productsContainer.GenerateDictionary();
        recycleRuleDict = recycleRule.GenerateDictionary();
        recycleHintDict = recycleHint.GenerateDictionary();
    }


    public GameObject GetProduct(ItemType type)
    {
        if (separateProductDict == null || !separateProductDict.ContainsKey(type)) return null;
        return separateProductDict[type];
    }


    public bool CheckRecyclable(ItemType itemType, RecycleType recycleType) =>
        recycleRuleDict[recycleType].ItemTypes.Contains(itemType);


    public RecycleType GetRecycleType(ItemType itemType)
    {
        return recycleRuleDict.Keys.FirstOrDefault(recycleType => recycleRuleDict[recycleType].ItemTypes.Contains(itemType));
    }
    

    public Sprite GetRecycleHint(RecycleType recycleType)
    {
        if (recycleHintDict == null || !recycleHintDict.ContainsKey(recycleType)) return null;
        return recycleHintDict[recycleType];
    }
}
