
public interface INormalSeparatorState
{
    NormalSeparatorMachine Machine { get; }
    void Enter();
    void Stay();
    void Exit();
}