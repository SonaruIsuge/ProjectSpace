using System;
using System.Collections.Generic;
using System.Linq;
using SonaruUtilities;
using UnityEngine;
using Random = UnityEngine.Random;


public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemContainer itemContainer;
    [SerializeField] private List<Transform> initItemPoint;
    [SerializeField] private List<Item> allWaitingItem;
    [SerializeField] private List<Item> allItemInStage;
    [SerializeField] private float popItemTime;
    [SerializeField] private float maxItemSpeed;

    private SimpleTimer timer;
    private Queue<Item> waitingQueue;

    public int ItemInStageNum => allItemInStage.Count;
    public int RemainItemNum => allItemInStage.Count(i => i.ItemData.type != ItemType.Energy);
    public event Action<Item, Player> OnItemStartInteract;
    public event Action<Item, Player> OnItemEndInteract;

    private bool isStart;
    public void SetStart(bool start) => isStart = start;
    

    private void Awake()
    {
        allItemInStage = FindObjectsOfType<Item>().ToList();
        itemContainer.GenerateDictionary();

        waitingQueue = new Queue<Item>(allWaitingItem);
        timer = new SimpleTimer(popItemTime);
        timer.Pause();
    }


    private void OnEnable()
    {
        foreach (var item in allItemInStage)
        {
            RegisterItemEvent(item);
        }
    }


    private void Update()
    {
        if(!isStart) return;

        if (timer.IsFinish && waitingQueue.Count > 0)
        {
            SpawnNewItem(waitingQueue.Dequeue());
            timer.Reset();
        }

        foreach (var item in allItemInStage.Where(item => item.Rb.velocity.magnitude > maxItemSpeed))
        {
            item.Rb.velocity = item.Rb.velocity.normalized * maxItemSpeed;
        }
    }


    public void InitialSetUp()
    {
        timer.Resume();
        foreach (var item in allItemInStage) item.Rb.velocity = Random.onUnitSphere;
    }

    
    public void RegisterItemEvent(Item newItem)
    {
        newItem.OnNewPlayerInteract += ItemStartInteract;
        newItem.OnRemovePlayerInteract += ItemEndInteract;
        newItem.OnItemRemove += ItemRemove;
        newItem.OnItemAppear += ItemAppear;
    }


    [ContextMenu("Clear All Item")]
    public void ClearAllItem()
    {
        for(var i = allItemInStage.Count - 1; i >= 0; i--) allItemInStage[i].RemoveItem();
    }


    private void ItemStartInteract(Item item, Player picker) => OnItemStartInteract?.Invoke(item, picker);
    private void ItemEndInteract(Item item, Player dropper) => OnItemEndInteract?.Invoke(item, dropper);


    private void ItemRemove(Item item)
    {
        if(!allItemInStage.Contains(item)) return;
        allItemInStage.Remove(item);
    }


    private void ItemAppear(Item newItem)
    {
        allItemInStage.Add(newItem);
    }


    private void SpawnNewItem(Item item)
    {
        var newItem = Instantiate(item);
        allWaitingItem.RemoveAt(0);
        RegisterItemEvent(newItem);
        newItem.AddItem();

        var newItemTrans = newItem.transform;
        newItemTrans.position = initItemPoint[Random.Range(0, initItemPoint.Count)].position;
        newItemTrans.rotation = Random.rotation;
        var velocity = (Vector3.zero - newItemTrans.position).normalized;
        velocity.x *= Random.Range(1f, 2f);
        velocity.y *= Random.Range(1f, 2f);
        velocity.z *= Random.Range(1f, 2f);
        newItem.Rb.velocity = velocity;
    }
}
