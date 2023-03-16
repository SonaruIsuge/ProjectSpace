using System.Collections.Generic;
using UnityEngine;


public class RecycleMachine : MonoBehaviour, IMachine
{
    [SerializeField] private RecycleType recycleType;
    [field: SerializeField] public Transform InputPoint { get; private set; }
    [field: SerializeField] public Vector3 InputRange { get; private set; }
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
        var results = new Collider[10];
        var hit = Physics.OverlapBoxNonAlloc(InputPoint.position, InputRange / 2f, results);
        if(hit == 0) return;

        Item targetItem = null;
        foreach (var result in results)
        {
            if(!result || !result.TryGetComponent<Item>(out var item)) continue;
            if(!DataManager.Instance.CheckRecyclable(item.ItemData.type, recycleType)) continue;

            targetItem = item;
        }
        
        if(!targetItem) return;
        
        // Recycle success
        if(targetItem.isInteract) targetItem.ForceDisconnect();
        recycleItem.Push(targetItem.transform.gameObject);
        targetItem.transform.SetParent(transform);
        targetItem.RemoveItem();

        FXController.Instance.InitVFX(VFXType.Recycle, InputPoint.position);
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(InputPoint.position, InputRange);
    }
}
