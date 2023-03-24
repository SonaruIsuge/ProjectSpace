
using System.Collections.Generic;
using System.Linq;
using Script.Item__Rubbish_;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataManager : TSingletonMonoBehaviour<DataManager>
{
    [SerializeField] private ItemContainer itemContainer;
    [SerializeField] private RecycleRule recycleRule;
    [SerializeField] private RecycleHintContainer recycleHint;

    private Dictionary<ItemType, GameObject> itemContainerDict;
    private Dictionary<RecycleType, ItemTypeCollection> recycleRuleDict;
    private Dictionary<RecycleType, Sprite> recycleHintDict;

    public List<InputDevice> AllPairedDevices { get; private set; } 

    public Material SelectItemMat;

    protected override void Awake()
    {
        base.Awake();
        itemContainerDict = itemContainer.GenerateDictionary();
        recycleRuleDict = recycleRule.GenerateDictionary();
        recycleHintDict = recycleHint.GenerateDictionary();

        AllPairedDevices = new List<InputDevice>();
    }


    public GameObject GetItem(ItemType type)
    {
        if (itemContainerDict == null || !itemContainerDict.ContainsKey(type)) return null;
        return itemContainerDict[type];
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


    public void SavePairedPlayer(List<PlayerPairingUnit> allPairedUnits)
    {
        foreach(var unit in allPairedUnits) AllPairedDevices.Add(unit.InputDevice);
    }
}
