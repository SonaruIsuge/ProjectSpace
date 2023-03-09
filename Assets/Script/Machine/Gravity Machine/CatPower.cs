using System;
using System.Collections.Generic;
using UnityEngine;


public class CatPower : MonoBehaviour, IPowerConsumable
{
    [field: SerializeField] public float MaxPower { get; private set; }
    [field: SerializeField] public float ConsumptionRate { get; private set; }

    [SerializeField] private List<Transform> energySlots;
    private Item[] energyInUse;
    private Item energyToCharge;

    private float remainPower;
    public float RemainPower
    {
        get => remainPower;
        private set => remainPower = Mathf.Clamp(value, 0, MaxPower);
    }

    public float RemainPowerPercent => RemainPower / MaxPower;
    public bool IsRemainPower => RemainPower > 0;

    public event Action<float> OnPowerConsume;
    public event Action OnPowerRunOut;
    public event Action OnPowerCharged;
    
    [field: Header("UI for Debug")]
    [field: SerializeField] public RectTransform PowerBg { get; private set; }
    [field: SerializeField] public RectTransform RemainPowerBar { get; private set; }


    public void InitialSetup()
    {
        remainPower = MaxPower;
        
        energyInUse = new Item[energySlots.Count];
        
        for (var i = 0; i < energySlots.Count; i++)
        {
            var energy = GetDetectEnergySlot(energySlots[i]);
            if(energy) energy.SetInteractable(false);
            energyInUse[i] = energy;
        }

        energyToCharge = null;
    }
    
    
    public void ConsumePower()
    {
        RemainPower -= ConsumptionRate * Time.deltaTime;
        
        if(!IsRemainPower) OnPowerRunOut?.Invoke();
        else OnPowerConsume?.Invoke(RemainPower);
        
        var oneEnergyPower = 1f / energySlots.Count;
        var remainNum = Mathf.CeilToInt(RemainPowerPercent / oneEnergyPower);
        if (remainNum < energySlots.Count)
        {
            var energy = energyInUse[remainNum];
            if (energy)
            {
                energyInUse[remainNum] = null;
                energy.RemoveItem();
            }
        }

        UpdateUI();
    }

    public void ChargePower()
    {
        for (var i = 0; i < energyInUse.Length; i++)
        {
            if ( energyInUse[i] != null ) continue;
            
            energyInUse[i] = energyToCharge;
            energyToCharge.ForceDisconnect();
                
            energyToCharge.transform.SetParent(energySlots[i]);
            energyToCharge.transform.localPosition = Vector3.forward * -0.05f;
            energyToCharge.transform.localRotation = Quaternion.Euler(Vector3.right * -90f);
            energyToCharge.transform.localScale = Vector3.one * .5f;
            energyToCharge.SetInteractable(false);
            energyToCharge.Rb.isKinematic = true;
                
            RemainPower += MaxPower / energySlots.Count;
            energyToCharge = null;
            
            OnPowerCharged?.Invoke();
            
            UpdateUI();
            
            return;
        }
    }


    private Item GetDetectEnergySlot(Transform slot)
    {
        if (!Physics.Raycast(slot.position, -slot.forward, out var hit)) return null;
        if (!hit.transform.TryGetComponent<Item>(out var item)) return null;
        return item.ItemData.type == ItemType.Energy ? item : null;
    }


    private void OnCollisionEnter(Collision col)
    {
        if(!col.transform.TryGetComponent<Item>(out var item)) return;
        if(item.ItemData.type != ItemType.Energy) return;

        energyToCharge = item;
        ChargePower();
    }


    private void UpdateUI()
    {
        var percentWidth = RemainPowerPercent * PowerBg.rect.width;
        RemainPowerBar.sizeDelta = new Vector2(percentWidth, RemainPowerBar.rect.height);
    }
}
