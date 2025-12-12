using Infrastructure.StateMachine;

namespace Player.States
{

// State when the player is climbing a ladder or similar object. Not implemented yet. Likely ever.
public class Climbing: PlayerState
{
    public Climbing(PlayerController p): base(p) { }
}

}
