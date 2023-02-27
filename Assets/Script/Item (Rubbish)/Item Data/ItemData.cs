﻿using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Item(Rubbish) System/Item")]
public class ItemData : ScriptableObject
{
    public ItemSize Size;
    public RecycleType RecycleType;
    
    // Random Choose the number of products list
    public int ProductNumber;
    public List<ItemData> SeparateProducets;
    
}
