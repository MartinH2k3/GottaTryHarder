using Infrastructure.StateMachine;

namespace Player.States
{
public class Walking: State
{
    private readonly PlayerController _player;

    public Walking(PlayerController player) {
        _player = player;
    }


}
}