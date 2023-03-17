using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Item(Rubbish) System/Item")]
public class ItemData : ScriptableObject
{
    public ItemType type;
    public ItemSize Size;
    
    // Random Choose the number of products list
    public int ProductNumber;
    public List<ItemData> SeparateProducets;
    
}
