
using SonaruUtilities;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRotateCamIcon : MonoBehaviour
{
    [SerializeField] private float existTime;
    [SerializeField] private RawImage iconImg;

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
    }


    public void Show(Vector2 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        
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
