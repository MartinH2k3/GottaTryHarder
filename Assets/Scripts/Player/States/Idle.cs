using Infrastructure.StateMachine;

namespace Player.States
{
public class Idle: State
{
    private readonly PlayerController _player;

    public Idle(PlayerController player) {
        _player = player;
    }
}
}