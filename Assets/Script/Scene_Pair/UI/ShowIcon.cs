using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShowIcon : MonoBehaviour
{
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private RawImage yesIcon;
    [SerializeField] private RawImage noIcon;
        
    private EventSystem EventSystem => EventSystem.current;
    
    
    private void Update()
    {
        if (EventSystem.currentSelectedGameObject == yesBtn.gameObject)
        {
            yesIcon.gameObject.SetActive(true);
            noIcon.gameObject.SetActive(false);
        }

        if (EventSystem.currentSelectedGameObject == noBtn.gameObject)
        {
            yesIcon.gameObject.SetActive(false);
            noIcon.gameObject.SetActive(true);
        }
    }
}
