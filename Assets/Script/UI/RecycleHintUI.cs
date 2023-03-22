
using System;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.UI;

public class RecycleHintUI : MonoBehaviour
{
    [SerializeField] private float ExistTime;
    [SerializeField] private Image hintImage;
    [SerializeField] private Animator hintShowAni;
    
    private SimpleTimer existTimer;
    private Player bindPlayer;
    private Item hintItem;
    private Camera mainCam => Camera.main;


    private void Awake()
    {
        existTimer = new SimpleTimer(ExistTime);
        existTimer.Pause();
        Hide();
    }


    private void Update()
    {
        if(existTimer.IsFinish) Hide();
        
        transform.position = mainCam.WorldToScreenPoint(bindPlayer.HeadPoint.position);
    }


    private void SetRecycleHintSprite(Sprite sprite) => hintImage.sprite = sprite;
    
    
    public void BindPlayer(Player player) => bindPlayer = player;


    public void SetHintItem(Item item)
    {
        hintItem = item;
        if(!item) return;
        
        SetRecycleHintSprite(DataManager.Instance.GetRecycleHint(DataManager.Instance.GetRecycleType(item.ItemData.type)));

        item.OnRemovePlayerInteract += HideHint;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        transform.position = mainCam.WorldToScreenPoint(bindPlayer.transform.position);
        hintShowAni.Play("ItemHint");
        existTimer.Reset();
    }


    public void Hide()
    {
        SetRecycleHintSprite(null);
        gameObject.SetActive(false);
    }


    private void HideHint(Item item, Player player)
    {
        if(player != bindPlayer) return;
        
        hintItem.OnRemovePlayerInteract -= HideHint;
        Hide();
    }
}
