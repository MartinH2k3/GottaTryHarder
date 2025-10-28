using Mechanics;
using Player.Stats;
using UnityEngine;

namespace Player.States
{
public class WallSliding: PlayerState
{
    private WallSlideStats _stats;

    public WallSliding(PlayerController p) : base(p) {
        _stats = P.wallSlideStats;
    }


    public override void Enter()
    {
        P.animator.SetBool("WallSliding", true);
    }

    public override void Exit() {
        base.Exit();
        P.animator.SetBool("WallSliding", false);
    }

    public override void FixedTick() {
        P.ResetCoyote();

        var v = P.GetVelocity();

        // Clamp sliding speed
        var wallSlideSpeed = -Mathf.Abs(_stats.wallSlideSpeed); // Make sure it's always negative no matter the input in game editor
        if (v.y < wallSlideSpeed)
            v.y = wallSlideSpeed;

        // Slow down to don't continuously push into wall
        v.x = Mathf.MoveTowards(v.x, 0f, _stats.wallSlideAccelX * Time.fixedDeltaTime);
        P.SetVelocity(v);

        // Wall jump
        if (P.ShouldStartJump) {
            WallJump();
            return;
        }

        var intent = P.Intent;
        // if moving towards the wall, do nothing
        if (P.WallDir * intent.Move.x >= 0f)
            return;

        // Go away from the wall
        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        var targetSpeed = horizontalInput * P.jumpStats.airSpeed;
        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? P.jumpStats.airAccel : P.jumpStats.airDecel;

        P.AccelerateX(targetSpeed, acceleration);
    }

    private void WallJump() {
        P.animator.SetTrigger("Jump");
        P.SetVelocity(Vector2.zero);
        P.AddForce(-P.WallDir * _stats.wallJumpXStrength, _stats.wallJumpYStrength, ForceMode2D.Impulse);

        P.ConsumeBufferedJump();
        P.ResetJumpCooldown();
        P.ConsumeCoyote();
        P.StartWallRegrabLock();
        P.StartWallJumpControlLock();
    }

}
}