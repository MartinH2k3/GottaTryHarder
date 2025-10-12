using MyPhysics;
using UnityEngine;

namespace Player.States
{
public class WallSliding: PlayerState
{
    private float _stickTimer;

    public WallSliding(PlayerController p): base(p) { }


    public override void Enter()
    {
        _stickTimer = P.wallStickTime;
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
        var wallSlideSpeed = -Mathf.Abs(P.wallSlideSpeed); // Make sure it's always negative no matter the input in game editor
        if (v.y < wallSlideSpeed)
            v.y = wallSlideSpeed;

        // Slow down to don't continuously push into wall
        v.x = Mathf.MoveTowards(v.x, 0f, P.wallSlideAccelX * Time.fixedDeltaTime);
        P.SetVelocity(v);

        // Wall jump
        if (P.ShouldStartJump) {
            WallJump();
            return;
        }

        if (P.PressingIntoWall()) _stickTimer = P.wallStickTime;
        else _stickTimer -= Time.fixedDeltaTime;
    }

    private void WallJump() {
        P.animator.SetTrigger("Jump");
        P.SetVelocity(Vector2.zero);
        P.AddForce(-P.WallDir * P.wallJumpXStrength, P.wallJumpYStrength, ForceMode2D.Impulse);

        P.ConsumeBufferedJump();
        P.ResetJumpCooldown();
        P.ConsumeCoyote();
        P.StartWallRegrabLock();
        P.StartWallJumpControlLock();
    }

    // Helper for transitions to check if still “sticky”
    public bool StickActive => _stickTimer > 0f;
}
}