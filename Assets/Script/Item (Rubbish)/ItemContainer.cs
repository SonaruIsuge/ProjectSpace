using System;
using System.Collections.Generic;
using SonaruUtilities;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Container", menuName = "Item(Rubbish) System/Container")]
public class ItemContainer : KeyValueScriptableObject<ItemType, GameObject> { } 
