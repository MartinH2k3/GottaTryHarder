using Mechanics;
using UnityEngine;

namespace Player.States
{
public class Idle: GroundedBase
{
    public Idle(PlayerController p) : base(p) { }

    public override void Enter() {
        base.Enter();
        P.animator.SetTrigger("Idle");
    }

    // Slow down to a stop when idle
    public override void FixedTick() {
        var v = P.GetVelocity();
        if (Mathf.Approximately(v.x, 0f)) {
            return;
        }
        var newX = Mathf.MoveTowards(v.x, 0f, P.movementStats.walkDecel * Time.fixedDeltaTime);
        P.SetVelocity(newX, v.y);
    }
}
}