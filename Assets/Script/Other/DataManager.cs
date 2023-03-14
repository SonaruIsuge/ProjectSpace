
using SonaruUtilities;
using UnityEngine;

public class DataManager : TSingletonMonoBehaviour<DataManager>
{
    [SerializeField] private ItemContainer itemContainer;
    public Material SelectItemMat;

    protected override void Awake()
    {
        base.Awake();
        itemContainer.GenerateDictionary();
    }


    public GameObject GetItem(ItemType type) => itemContainer.GetItemByType(type);
}
