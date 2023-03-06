


public interface IGravityAffectable
{
    bool UnderGravity { get; set; }
    bool IgnoreGravity { get; }

    void ApplyGravity(float gravity);
}
