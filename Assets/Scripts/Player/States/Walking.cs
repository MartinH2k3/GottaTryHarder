using MyPhysics;
using UnityEngine;

namespace Player.States
{
public class Walking: GroundedBase
{
    public Walking(PlayerController p): base(p) { }

    public override void FixedTick() {
        var intent = P.Intent;
        if (Mathf.Approximately(intent.Move.x, 0)) return;

        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        var targetSpeed = horizontalInput *
                          P.walkSpeed *
                          (intent.SprintHeld ? P.sprintMultiplier : 1f) *
                          P.WalkSpeedMultiplier;

        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? P.walkAccel : P.walkDecel;

        P.AccelerateHorizontally(targetSpeed, acceleration, horizontalInput);
    }

}
}