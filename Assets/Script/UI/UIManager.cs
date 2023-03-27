using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using SonaruUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Dictionary<Player, RecycleHintUI> playerHintUIDict;
    private Dictionary<Player, PlayerRotateCamIcon> playerIconDict;

    [Header("In Game")] 
    [SerializeField] private MainGameView mainGameView;
    [SerializeField] private RecycleHintUI recycleHintUIPrefab;
    [SerializeField] private PlayerRotateCamIcon playerRotateCamPrefab;
    [SerializeField] private List<Texture> allIconTextures;
    [Header("Game Over")]
    [SerializeField] private GameOverView gameOverView;

    private int pairPlayerNum;

    public event Action OnPressReplay;

    private void Awake()
    {
        pairPlayerNum = 0;
        
        mainGameView.gameObject.SetActive(false);
        gameOverView.gameObject.SetActive(false);
        
        playerHintUIDict = new Dictionary<Player, RecycleHintUI>();
        playerIconDict = new Dictionary<Player, PlayerRotateCamIcon>();
    }


    private void OnEnable()
    {
        ItemManager.OnItemStartInteract += ShowItemHint;

        gameOverView.BindReplayButton(() => OnPressReplay?.Invoke() );
    }


    private void OnDisable()
    {
        ItemManager.OnItemStartInteract -= ShowItemHint;
    }


    public void BindActivePlayerUI(Player player, int characterIndex)
    {
        pairPlayerNum++;
        var hintUI = Instantiate(recycleHintUIPrefab, mainGameView.transform);
        hintUI.BindPlayer(player);
        playerHintUIDict.Add(player, hintUI);

        var playerIcon = Instantiate(playerRotateCamPrefab, mainGameView.transform);
        playerIcon.BindPlayerWithIcon(player, allIconTextures[characterIndex]);
        playerIconDict.Add(player, playerIcon);
    }


    public void UpdateItemRemain(int remain)
    {
        mainGameView.SetRemainRubbish(remain);   
    }


    public void UpdateTimeRemain(float remain, float remain01)
    {
        mainGameView.SetRemainTime(remain, remain01);
    }


    public void SetGameStartUI()
    {
        mainGameView.gameObject.SetActive(true);
        gameOverView.gameObject.SetActive(false);
    }


    public void SetGameOverUI(bool isWin, float useTime)
    {
        gameOverView.gameObject.SetActive(true);
        gameOverView.SetGameOverData(isWin, useTime);
    }


    public void ShowPlayerIcon(Player player, float clockwise)
    {
        playerIconDict[player].Show(mainGameView.RandomFromRotateUI(clockwise));
    }


    private void ShowItemHint(Item item, Player player)
    {
        var hintUI = playerHintUIDict[player];
        hintUI.SetHintItem(item);
        hintUI.Show();
    }
}
