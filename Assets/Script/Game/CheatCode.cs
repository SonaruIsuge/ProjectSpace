using SonaruUtilities;
using UnityEngine;
using UnityEngine.InputSystem;


public class CheatCode : MonoBehaviour
{
    private ItemManager itemManager;
    private SimpleTimer cheatPressTimer;

    private bool cheatUsed;

    private void Awake()
    {
        itemManager = FindObjectOfType<ItemManager>();
        
        cheatPressTimer = new SimpleTimer(3);
        cheatPressTimer.Pause();

        cheatUsed = false;
    }
    

    private void Update()
    {
        if (cheatUsed) return;
        
        if (Keyboard.current.kKey.isPressed) cheatPressTimer.Resume();
        else cheatPressTimer.Reset();

        if (!cheatPressTimer.IsFinish) return;
        itemManager.ClearAllItem();
        cheatPressTimer.Pause();
        cheatUsed = true;
    }
}