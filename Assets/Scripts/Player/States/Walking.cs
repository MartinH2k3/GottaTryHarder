using Mechanics;
using UnityEngine;

namespace Player.States
{
public class Walking: GroundedBase
{
    public Walking(PlayerController p): base(p) { }

    public override void Enter() {
        base.Enter();
        P.animator.SetTrigger("Walk");
    }

    public override void FixedTick() {
        var intent = P.Intent;
        if (Mathf.Approximately(intent.Move.x, 0)) return;

        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        if (intent.SprintHeld)
            P.animator.SetBool("Sprinting", true);
        else
            P.animator.SetBool("Sprinting", false);

        var targetSpeed = horizontalInput *
                          P.walkSpeed *
                          (intent.SprintHeld ? P.sprintMultiplier : 1f) *
                          P.WalkSpeedMultiplier;

        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? P.walkAccel : P.walkDecel;

        P.AccelerateHorizontally(targetSpeed, acceleration, horizontalInput);
    }

}
}