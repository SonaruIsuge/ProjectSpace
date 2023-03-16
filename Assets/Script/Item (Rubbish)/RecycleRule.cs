using System.Collections.Generic;
using SonaruUtilities;
using UnityEngine;

namespace Script.Item__Rubbish_
{
    [CreateAssetMenu(fileName = "Recycle Rule", menuName = "Item(Rubbish) System/Recycle Rule")]
    public class RecycleRule : KeyValueScriptableObject<RecycleType, ItemTypeCollection> { }
}


[System.Serializable]
public struct ItemTypeCollection
{
    public List<ItemType> ItemTypes;
}