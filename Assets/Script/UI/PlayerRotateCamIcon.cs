
using SonaruUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotateCamIcon : MonoBehaviour
{
    [SerializeField] private float existTime;
    [SerializeField] private RawImage iconImg;
    [SerializeField] private TMP_Text playerIndex;
    [SerializeField] private Player bindPlayer;
    private Texture icon;
    private SimpleTimer existTimer;


    private void Awake()
    {
        existTimer = new SimpleTimer(existTime);
        existTimer.Pause();
        Hide();
    }


    private void Update()
    {
        if(existTimer.IsFinish) Hide();
    }


    public void BindPlayerWithIcon(Player player, Texture texture)
    {
        bindPlayer = player;
        icon = texture;
        iconImg.texture = icon;
        playerIndex.text = $"P{player.PlayerIndex + 1}";
    }


    public void Show(Vector2 pos, bool isLeft)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        
        var textPos = playerIndex.rectTransform.anchoredPosition;
        textPos.x = isLeft ? 510 : -510;
        playerIndex.rectTransform.anchoredPosition = textPos;
        
        existTimer.Reset();
        existTimer.Resume();
    }


    public void Hide()
    {
        gameObject.SetActive(false);
        
        existTimer.Reset();
        existTimer.Pause();
    }
}
