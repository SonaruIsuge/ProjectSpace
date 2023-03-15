using System;
using UnityEngine;



public class CatPower : MonoBehaviour, IPowerConsumable
{
    [field: SerializeField] public float MaxPower { get; private set; }
    [field: SerializeField] public float ConsumptionRate { get; private set; }
    [SerializeField] private float chargeRate;
    private Item chargeJam;
    
    private float remainPower;
    public float RemainPower
    {
        get => remainPower;
        private set => remainPower = Mathf.Clamp(value, 0, MaxPower);
    }

    public float RemainPowerPercent => RemainPower / MaxPower;
    public bool IsRemainPower => RemainPower > 0;
    
    public event Action OnPowerRunOut;
    public event Action<float> OnPowerConsume;
    public event Action OnPowerCharged;
    
    [field: Header("UI for Debug")]
    [field: SerializeField] public RectTransform PowerBg { get; private set; }
    [field: SerializeField] public RectTransform RemainPowerBar { get; private set; }
    
    
    public void InitialSetup()
    {
        remainPower = MaxPower;
        chargeJam = null;
    }
    

    public void ConsumePower()
    {
        RemainPower -= ConsumptionRate * Time.deltaTime;
        
        if(!IsRemainPower) OnPowerRunOut?.Invoke();
        else OnPowerConsume?.Invoke(RemainPower);
        
        UpdateUI();
    }

    
    public void ChargePower()
    {
        if(chargeJam.isInteract) chargeJam.ForceDisconnect();
        chargeJam.RemoveItem();
    }
    
    
    private void OnCollisionEnter(Collision col)
    {
        if(!col.transform.TryGetComponent<Item>(out var item)) return;
        if(item.ItemData.type != ItemType.Energy) return;

        chargeJam = item;
        ChargePower();
        RemainPower += chargeRate;
        chargeJam = null;
            
        OnPowerCharged?.Invoke();
        UpdateUI();
    }
    
    
    private void UpdateUI()
    {
        var percentWidth = RemainPowerPercent * PowerBg.rect.width;
        RemainPowerBar.sizeDelta = new Vector2(percentWidth, RemainPowerBar.rect.height);
    }
}
