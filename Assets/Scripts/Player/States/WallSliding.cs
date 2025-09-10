using Infrastructure.StateMachine;

namespace Player.States
{
public class WallSliding: State
{
    private readonly PlayerController _player;

    public WallSliding(PlayerController player)
    {
        _player = player;
    }
}
}