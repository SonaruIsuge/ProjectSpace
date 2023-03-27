
using System;
using System.Threading.Tasks;
using DG.Tweening;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RecycleHintUI : MonoBehaviour
{
    [SerializeField] private float existTime;
    [SerializeField] private float fadeTime;
    [SerializeField] private Image hintImage;
    
    private SimpleTimer existTimer;
    private Tweener showImgTween;
    private Player bindPlayer;
    private Item hintItem;
    private Camera MainCam => Camera.main;


    private void Awake()
    {
        existTimer = new SimpleTimer(existTime);
        existTimer.Pause();
        Hide(false);
    }


    private void Update()
    {
        if(existTimer.IsFinish) Hide();
        
        transform.position = MainCam.WorldToScreenPoint(bindPlayer.HeadPoint.position);
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
        transform.position = MainCam.WorldToScreenPoint(bindPlayer.transform.position);
        
        var hintImgColor = hintImage.color;
        hintImgColor.a = 1f;
        showImgTween?.Kill();
        showImgTween = hintImage.DOColor(hintImgColor, fadeTime);
        showImgTween.Play();
        
        existTimer.Resume();
    }


    public void Hide(bool useFade = true)
    {
        var hintImgColor = hintImage.color;
        hintImgColor.a = 0f;
        showImgTween?.Kill();
        if (useFade)
        {
            showImgTween = hintImage.DOColor(hintImgColor, fadeTime).OnComplete(() => SetRecycleHintSprite(null));
            showImgTween.Play();
        }
        else
        {
            hintImage.color = hintImgColor;
        }
        
        existTimer.Reset();
        existTimer.Pause();
    }


    private void HideHint(Item item, Player player)
    {
        if(player != bindPlayer) return;
        
        hintItem.OnRemovePlayerInteract -= HideHint;
        Hide();
    }
}
