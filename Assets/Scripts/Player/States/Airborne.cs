using Infrastructure.StateMachine;

namespace Player.States
{
public class Airborne: State
{
    private readonly PlayerController _player;

    public Airborne(PlayerController player)
    {
        _player = player;
    }
}
}