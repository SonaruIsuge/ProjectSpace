
public interface IMachine
{
    bool IsActive { get; }
    bool IsWorking { get; }
    bool IsBroken { get; }

    void SetActive(bool active);
    void SetUp();
    void Work();
    void GetDamage();
}
