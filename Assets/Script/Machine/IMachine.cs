
public interface IMachine
{
    bool IsWorking { get; }
    bool IsBroken { get; }

    void Work();
    void GetDamage();
}
