using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectBox : MonoBehaviour
{
    private GameObject CurrentSelected => EventSystem.current.currentSelectedGameObject;
    private GameObject nowRecordSelected;


    private void OnEnable()
    {
        nowRecordSelected = CurrentSelected;
    }


    private void Update()
    {
        if (!CurrentSelected || nowRecordSelected == CurrentSelected) return;
        
        nowRecordSelected = CurrentSelected;
        transform.position = nowRecordSelected.transform.position;
    }
}
