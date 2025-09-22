using MyPhysics;

namespace Player.States
{
public class Dashing: PlayerState
{
    private int _dashDirection;
    private float _ogGravity;

    public Dashing(PlayerController p) : base(p) { }

    public override void Enter() {
        _ogGravity = P.GetGravityScale();
        P.SetGravityScale(0f);
        _dashDirection = P.FacingDirection;
    }

    public override void FixedTick() {
        P.SetVelocity(_dashDirection * P.dashSpeed, 0);
    }

    public override void Exit() {
        P.SetGravityScale(_ogGravity);
        P.StartDashCooldown();
    }


}
}