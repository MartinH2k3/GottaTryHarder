using Physics;
using UnityEngine;

namespace Player.States
{
public class Airborne: PlayerState
{
    public Airborne(PlayerController p) : base(p) { }

    public override void FixedTick() {
        if (P.ShouldStartJump) Jump();

        var intent = P.Intent;
        if (Mathf.Approximately(intent.Move.x, 0)) return;

        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        var targetSpeed = horizontalInput *
                          P.walkSpeed *
                          (intent.SprintHeld && P.allowSprintInAir ? P.sprintMultiplier : 1f);
        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? P.airAccel : P.airDecel;

        P.AccelerateHorizontally(targetSpeed, acceleration, horizontalInput);
    }

    private void Jump() {
        P.ConsumeBufferedJump();

        if (!P.CanSingleJump && !P.CanAirJump)
            return;

        if (!P.CanSingleJump && P.CanAirJump)
            P.ConsumeAirJump();

        if (P.GetVelocity().y < 0f)
            P.SetVelocityY(0f); // Jumping stops falling

        P.AddForce(Vector2.up * P.jumpStrength, ForceMode2D.Impulse);
        P.ConsumeBufferedJump();
        P.ResetJumpCooldown();
        P.ConsumeCoyote();
    }
}
}