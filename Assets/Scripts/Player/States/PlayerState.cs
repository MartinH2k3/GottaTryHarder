using Infrastructure.StateMachine;

namespace Player.States
{
public abstract class PlayerState: State
{
    protected readonly PlayerController P;

    protected PlayerState(PlayerController p) => P = p;
}
}