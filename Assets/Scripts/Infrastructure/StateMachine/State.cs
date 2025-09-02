namespace Infrastructure.StateMachine
{
public abstract class State: IState
{
    public virtual void Enter() {

    }

    public virtual void Tick() {

    }

    public virtual void FixedTick() {

    }

    public virtual void Exit() {

    }
}
}