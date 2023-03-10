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
    [SerializeField] private NormalSeparatorMachine normalSeparatorMachine;

    private SimpleTimer timer;
    private Queue<Item> waitingQueue;

    public static event Action<Item, Player> OnItemStartInteract;
    public static event Action<Item, Player> OnItemEndInteract;

    private bool isStart;
    public void SetStart(bool start) => isStart = start;
    

    private void Awake()
    {
        allItemInStage = FindObjectsOfType<Item>().ToList();
        itemContainer.GenerateDictionary();

        waitingQueue = new Queue<Item>(allWaitingItem);
        timer = new SimpleTimer(popItemTime);
        timer.Pause();
        isStart = false;
    }


    private void OnEnable()
    {
        foreach (var item in allItemInStage)
        {
            item.OnNewPlayerInteract += ItemStartInteract;
            item.OnRemovePlayerInteract += ItemEndInteract;
            item.OnItemRemove += ItemRemove;
            item.OnItemAppear += ItemAppear;
        }
        
        normalSeparatorMachine.OnNewItemOutput += ItemAppear;
    }


    private void Update()
    {
        if(!isStart) return;

        if (timer.IsFinish && waitingQueue.Count > 0)
        {
            InitNewItem(waitingQueue.Dequeue());
            timer.Reset();
        }
    }


    public void InitialSetUp()
    {
        timer.Resume();
        foreach (var item in allItemInStage) item.Rb.velocity = Random.onUnitSphere;
    }


    public GameObject GetItem(ItemType type) => itemContainer.GetItemByType(type);


    public int GetItemInStageNum() => allItemInStage.Count;


    [ContextMenu("Clear All Item")]
    public void ClearAllItem()
    {
        for(var i = allItemInStage.Count - 1; i >= 0; i--) allItemInStage[i].RemoveItem();
    }


    private void ItemStartInteract(Item item, Player picker) => OnItemStartInteract?.Invoke(item, picker);
    private void ItemEndInteract(Item item, Player dropper) => OnItemEndInteract?.Invoke(item, dropper);


    private void ItemRemove(Item inputItem)
    {
        if(!allItemInStage.Contains(inputItem)) return;
        allItemInStage.Remove(inputItem);
    }
    
    
    private void ItemAppear(Item outputItem)
    {
        allItemInStage.Add(outputItem);
        outputItem.OnNewPlayerInteract += ItemStartInteract;
        outputItem.OnRemovePlayerInteract += ItemEndInteract;
        outputItem.OnItemRemove += ItemRemove;
        outputItem.OnItemAppear += ItemAppear;
    }


    private void InitNewItem(Item item)
    {
        var newItem = Instantiate(item);
        newItem.transform.position = initItemPoint[Random.Range(0, initItemPoint.Count)].position;
        newItem.transform.rotation = Random.rotation;
        allWaitingItem.RemoveAt(0);
        ItemAppear(newItem);
        var velocity = (Vector3.zero - newItem.transform.position).normalized;
        velocity.x *= Random.Range(1f, 2f);
        velocity.y *= Random.Range(1f, 2f);
        velocity.z *= Random.Range(1f, 2f);
        newItem.Rb.velocity = velocity;
    }
}
