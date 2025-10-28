using Mechanics;
using Player.Stats;
using UnityEngine;

namespace Player.States
{
public class Airborne: PlayerState
{
    private JumpStats _stats;

    public Airborne(PlayerController p) : base(p) {
        _stats = P.jumpStats;
    }

    public override void Enter() {
        base.Enter();
        if (P.Intent.LastJumpPressedTime <= Time.time - 0.3f)
            P.animator.SetTrigger("Fall");
    }

    public override void FixedTick() {
        if (P.ShouldStartJump) Jump();

        var intent = P.Intent;
        if (Mathf.Approximately(intent.Move.x, 0)) return;

        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        var targetSpeed = horizontalInput *
                          _stats.airSpeed *
                          (intent.SprintHeld && _stats.allowSprintInAir ? P.sprintMultiplier : 1f);
        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? _stats.airAccel : _stats.airDecel;

        P.AccelerateHorizontally(targetSpeed, acceleration);
    }

    private void Jump() {
        if (!P.CanSingleJump && !P.CanAirJump)
            return;

        P.animator.SetTrigger("Jump");
        if (!P.CanSingleJump && P.CanAirJump)
            P.ConsumeAirJump();

        if (P.GetVelocity().y < 0f)
            P.SetVelocityY(0f); // Jumping stops falling

        P.AddForce(Vector2.up * _stats.jumpStrength, ForceMode2D.Impulse);
        P.ConsumeBufferedJump();
        P.ResetJumpCooldown();
        P.ConsumeCoyote();
        P.LastJumpTime = Time.time;
    }
}
}