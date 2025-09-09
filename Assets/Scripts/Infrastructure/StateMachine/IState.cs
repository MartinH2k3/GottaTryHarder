namespace Infrastructure.StateMachine
{

public interface IState
{
    /// <summary>Called once when entering the state. Duh.</summary>
    void Enter();

    /// <summary>Called every frame (called on MonoBehaviour.Update()).</summary>
    void Tick();

    /// <summary>Called on every frame after Tick (called on MonoBehaviour.LateUpdate()).</summary>
    void LateTick();

    /// <summary>Called on fixed time (called on MonoBehaviour.FixedUpdate()).</summary>
    void FixedTick();

    /// <summary>Called once when exiting the state. Duh.</summary>
    void Exit();

}

}

