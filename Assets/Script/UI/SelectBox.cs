using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectBox : MonoBehaviour
{
    [SerializeField] private float flashSpace;
    private GameObject CurrentSelected => EventSystem.current.currentSelectedGameObject;
    private GameObject nowRecordSelected;
    private RawImage image;


    private void Awake()
    {
        image = GetComponent<RawImage>();
    }


    private void OnEnable()
    {
        nowRecordSelected = CurrentSelected;
    }


    private void Update()
    {
        if (CurrentSelected)
        {
            if (nowRecordSelected != CurrentSelected)
            {
                if(nowRecordSelected) FXController.Instance.InitSfx(SFXType.ButtonChange);
                nowRecordSelected = CurrentSelected;
            }
            transform.position = CurrentSelected.transform.position;
        }
    }


    public async void PressButton()
    {
        var originColor = image.color;
        originColor.a = 0;
        image.color = originColor;
        
        var timer = 0f;
        while (timer < flashSpace)
        {
            timer += Time.deltaTime;
            await Task.Yield();
        }

        originColor.a = 1;
        image.color = originColor;
    }
}
