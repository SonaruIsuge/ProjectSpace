
using System;
using System.Collections.Generic;
using SonaruUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExtraScene : MonoBehaviour
{
    [SerializeField] private SceneIndex targetScene = SceneIndex.Universe;
    private SimpleTimer cheatPressTimer;

    private bool cheatUsed;

    private void Awake()
    {
        cheatPressTimer = new SimpleTimer(3);
        cheatPressTimer.Pause();
        cheatUsed = false;
    }
    

    private void Update()
    {
        if (cheatUsed) return;
        
        if (Keyboard.current.uKey.isPressed) cheatPressTimer.Resume();
        else cheatPressTimer.Reset();

        if (!cheatPressTimer.IsFinish) return;
        
        cheatPressTimer.Pause();
        cheatUsed = true;
        EnterScene();
    }
    
    
    private void EnterScene()
    {
        GameFlowManager.Instance.LoadScene((int)targetScene, null);
    }
}
