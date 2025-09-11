using Physics;
using UnityEngine;

namespace Player.States
{
public class Airborne: PlayerState
{
    public Airborne(PlayerController p) : base(p) { }

    public override void Enter() {
        Debug.Log("Entered Airborne State");
    }

    public override void FixedTick() {
        if (P.ShouldStartJump) Jump();

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
        P.ResetCoyote();
    }
}
}