using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class CatPower : MonoBehaviour, IPowerConsumable
{
    [field: SerializeField] public float MaxPower { get; private set; }
    [field: SerializeField] public float ConsumptionRate { get; private set; }
    [SerializeField] private float chargeRate;
    [SerializeField] private Renderer radiusRenderer;

    //private Material radiusMat;
    //private float default_BlinkAlphaSpeed;
    
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
    
    [field: Header("UI")] 
    [SerializeField] private Image remainPowerBar;
    
    
    public void InitialSetup()
    {
        remainPower = MaxPower;
        chargeJam = null;

        //radiusMat = radiusRenderer.material;
        //default_BlinkAlphaSpeed = radiusMat.GetFloat("_BlinkAlphaSpeed");
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
        
        //radiusMat.SetFloat("_BlinkAlphaSpeed", default_BlinkAlphaSpeed);
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
        //if (RemainPower / MaxPower <= 0.2) radiusMat.SetFloat("_BlinkAlphaSpeed", 10);
        
        remainPowerBar.fillAmount = RemainPower / MaxPower;
    }
}
