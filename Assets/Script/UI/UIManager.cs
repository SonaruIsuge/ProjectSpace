using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using SonaruUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Dictionary<Player, RecycleHintUI> playerHintUIDict;

    [Header("Before Game Play")]
    [SerializeField] private GameObject WaitToStartGamePanel;
    [SerializeField] private List<Camera> playerCameras;
    [SerializeField] private List<TMP_Text> playerReadyText;
    [Header("In Game")] 
    [SerializeField] private TMP_Text remainItemNumText;
    [SerializeField] private RecycleHintUI recycleHintUI;
    [Header("Game Over")]
    [SerializeField] private GameOverView gameOverView;

    private int pairPlayerNum;

    public event Action OnAllReadyUIFinish;
    public event Action OnPressReplay;

    private void Awake()
    {
        pairPlayerNum = 0;
        WaitToStartGamePanel.SetActive(true);
        gameOverView.gameObject.SetActive(false);
        
        playerHintUIDict = new Dictionary<Player, RecycleHintUI>();
    }


    private void OnEnable()
    {
        ItemManager.OnItemStartInteract += ShowItemHint;
    }


    private void OnDisable()
    {
        ItemManager.OnItemStartInteract -= ShowItemHint;
    }


    public void PlayerPair(Player player)
    {
        pairPlayerNum++;
        
        var hintUI = Instantiate(recycleHintUI, transform);
        playerHintUIDict.Add(player, hintUI);
        hintUI.BindPlayer(player);
        
        WaitToStartGamePanel.SetActive(false);
        
        playerCameras[pairPlayerNum - 1].gameObject.SetActive(true);
        UpdateAllCameraRect(pairPlayerNum);
    }


    public void PlayerReady(Player player, int playerIndex, bool isReady)
    {
        var targetText = playerReadyText[playerIndex];
        if (isReady)
        {
            targetText.gameObject.SetActive(true);
            targetText.rectTransform.position = 
                playerCameras[playerIndex].WorldToScreenPoint(player.HeadPoint.position);
        }
        else targetText.gameObject.SetActive(false);
    }


    public async void AllPlayerReady()
    {
        // Transition animation
        await Task.Delay(2000);
        
        foreach(var cam in playerCameras) cam.gameObject.SetActive(false);
        foreach(var text in playerReadyText) if(text != null) text.gameObject.SetActive(false);
        
        remainItemNumText.gameObject.SetActive(true);
        
        // game start
        OnAllReadyUIFinish?.Invoke();
    }
    

    private void UpdateAllCameraRect(int activeCamNum)
    {
        for (var i = 0; i < activeCamNum; i++)
        {
            var rect = playerCameras[i].rect;

            rect.x = i * (1f / activeCamNum);
            rect.width = 1f / activeCamNum;
            playerCameras[i].DORect(rect, .2f);
        }
    }

    public void UpdateItemRemainText(int remain) => remainItemNumText.text = remain.ToString();


    public void SetGameOverUI(bool isWin, float useTime)
    {
        //GameOverPanel.SetActive(true);
        gameOverView.gameObject.SetActive(true);
        gameOverView.SetGameOverData(isWin, useTime);
    }


    private void ShowItemHint(Item item, Player player)
    {
        var hintUI = playerHintUIDict[player];
        hintUI.SetHintItem(item);
        hintUI.Show();
    }
}
