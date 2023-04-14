using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<Player, RecycleHintUI> playerHintUIDict;

    [Header("Game Start")]
    [SerializeField] private GameStartView gameStartView;
    [Header("In Game")] 
    [SerializeField] private MainGameView mainGameView;
    [SerializeField] private RecycleHintUI recycleHintUIPrefab;

    [Header("Game Over")]
    [SerializeField] private GameOverView gameOverView;

    private int pairPlayerNum;

    public event Action OnPressBackToPair;
    public event Action OnPressReplay;
    public event Action OnPressNextLevel; 

    private void Awake()
    {
        pairPlayerNum = 0;
        
        mainGameView.gameObject.SetActive(false);
        gameOverView.gameObject.SetActive(false);
        
        playerHintUIDict = new Dictionary<Player, RecycleHintUI>();
    }


    private void OnEnable()
    {
        gameOverView.BindReplayButton(() => OnPressReplay?.Invoke() );
        gameOverView.BindQuitButton( () => OnPressBackToPair?.Invoke() );
        gameOverView.BindNextLevelButton(() => OnPressNextLevel?.Invoke() );
    }


    public void BindActivePlayerUI(Player player, int characterIndex)
    {
        pairPlayerNum++;
        var hintUI = Instantiate(recycleHintUIPrefab, mainGameView.transform);
        hintUI.BindPlayer(player);
        playerHintUIDict.Add(player, hintUI);
    }


    public void UpdateItemRemain(int remain)
    {
        mainGameView.SetRemainRubbish(remain);   
    }


    public void UpdateTimeRemain(float remain, float remain01)
    {
        mainGameView.SetRemainTime(remain, remain01);
    }


    public async Task ShowStartAni(float delay, Action onComplete = null)
    {
        gameStartView.gameObject.SetActive(true);
        gameStartView.ResetTween();
        await Task.Delay((int)(delay * 1000));
        await gameStartView.ShowAni(() =>
        {
            onComplete?.Invoke();
            gameOverView.gameObject.SetActive(false);
            gameStartView.gameObject.SetActive(false);
        });
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


    public void ShowItemHint(Item item, Player player)
    {
        var hintUI = playerHintUIDict[player];
        hintUI.SetHintItem(item);
        hintUI.Show();
    }
}
