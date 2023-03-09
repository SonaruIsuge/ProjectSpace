using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Before Game Play")]
    [SerializeField] private GameObject WaitToStartGamePanel;
    [SerializeField] private List<Camera> playerCameras;
    [SerializeField] private List<TMP_Text> playerReadyText;
    [Header("In Game")] 
    [SerializeField] private TMP_Text remainItemNumText;
    [Header("Game Over")]
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private Button ReplayBtn; 

    private int pairPlayerNum;

    public event Action OnAllReadyUIFinish;
    public event Action OnPressReplay;

    private void Awake()
    {
        pairPlayerNum = 0;
        WaitToStartGamePanel.SetActive(true);
    }


    private void OnEnable()
    {
        ReplayBtn.onClick.AddListener(() => OnPressReplay?.Invoke());
    }


    private void OnDisable()
    {
        ReplayBtn.onClick.RemoveAllListeners();
    }


    public void PlayerPair(Player player)
    {
        pairPlayerNum++;
        
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
                playerCameras[playerIndex].WorldToScreenPoint(player.transform.position + new Vector3(0, 2.5f, 0));
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


    public void SetGameOverUI()
    {
        GameOverPanel.SetActive(true);
    }
}
