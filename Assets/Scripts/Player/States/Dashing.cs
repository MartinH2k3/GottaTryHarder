using Mechanics;
using Player.Stats;

namespace Player.States
{
public class Dashing: PlayerState
{
    private DashStats _stats;
    private int _dashDirection;
    private float _ogGravity;

    public Dashing(PlayerController p) : base(p) {
        _stats = P.dashStats;
    }

    public override void Enter() {
        _ogGravity = P.GetGravityScale();
        P.SetGravityScale(0f);
        _dashDirection = P.FacingDirection;
    }

    public override void FixedTick() {
        P.SetVelocity(_dashDirection * _stats.dashSpeed, 0);
    }

    public override void Exit() {
        P.SetGravityScale(_ogGravity);
        P.StartDashCooldown();
        P.SetVelocityX(0);
    }


}
}