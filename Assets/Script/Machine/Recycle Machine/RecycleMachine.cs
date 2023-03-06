using System.Collections.Generic;
using UnityEngine;


public class RecycleMachine : MonoBehaviour, IMachine
{
    [SerializeField] private RecycleType recycleType;
    [field: SerializeField] public Transform InputPoint { get; private set; }
    private Stack<GameObject> recycleItem;
    public bool IsActive { get; private set; }
    public bool IsWorking { get; }
    public bool IsBroken { get; }
    
    
    public void SetActive(bool active) => IsActive = active;

    
    public void SetUp()
    {
        recycleItem = new Stack<GameObject>();
    }

    
    public void Work()
    {
        DetectRecyclable();
    }

    
    public void GetDamage()
    {
        
    }


    private void DetectRecyclable()
    {
        var hit = Physics.Raycast(InputPoint.position, InputPoint.up, out var hitInfo, 1f);
        if( !hit || !hitInfo.transform.TryGetComponent<Item>(out var item)) return;
        if(item.isInteract) return;
        if(item.ItemData.RecycleType != recycleType) return;
        
        recycleItem.Push(hitInfo.transform.gameObject);
        hitInfo.transform.SetParent(transform);
        item.RemoveItem();
    }
}
