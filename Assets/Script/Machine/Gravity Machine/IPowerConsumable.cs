
using System;


public interface IPowerConsumable
{
    float MaxPower { get; }
    float ConsumptionRate { get; }
    float RemainPower { get; }
    float RemainPowerPercent { get; }
    bool IsRemainPower { get; }

    event Action OnPowerRunOut;
    event Action OnPowerCharged;

    void InitialSetup();
    void ConsumePower();
    void ChargePower();
}
