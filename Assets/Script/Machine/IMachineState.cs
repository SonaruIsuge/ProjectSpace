

public interface IMachineState
{
    void Enter();
    void Stay();
    void Exit();
}



public interface INormalSeparatorState : IMachineState
{
    NormalSeparatorMachine Machine { get; }
}



public interface IGravityControlMachineState : IMachineState
{
    GravityControlMachine Machine { get; }
}


