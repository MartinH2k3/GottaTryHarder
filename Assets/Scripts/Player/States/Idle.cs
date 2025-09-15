using Physics;
using UnityEngine;

namespace Player.States
{
public class Idle: GroundedBase
{
    public Idle(PlayerController p) : base(p) { }

    public override void FixedTick() {
        var v = P.GetVelocity();
        if (Mathf.Approximately(v.x, 0f)) {
            return;
        }
        var newX = Mathf.MoveTowards(v.x, 0f, P.walkDecel * Time.fixedDeltaTime);
        P.SetVelocity(newX, v.y);
    }
}
}