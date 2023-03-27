
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PairingSceneUIManager : MonoBehaviour
{
    [SerializeField] private Transform canvasTrans;
    [SerializeField] private List<TMP_Text> allPairedPlayerText;
    
    private Camera MainCam => Camera.main;


    public void Init()
    {
        foreach(var readyText in allPairedPlayerText) readyText.gameObject.SetActive(false);
    }
    

    public void PlayerChangeReady(int index, bool isReady)
    {
        allPairedPlayerText[index].gameObject.SetActive(isReady);
    }
}
