// /AI/Core/StateMachine.cs
public interface IState { void Enter(); void Tick(float dt); void Exit(); }
public sealed class StateMachine
{
    IState cur;
    public void Set(IState next) { if (next == cur) return; cur?.Exit(); cur = next; cur?.Enter(); }
    public void Tick(float dt) { cur?.Tick(dt); }
}
