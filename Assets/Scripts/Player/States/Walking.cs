using Player.Stats;
using Mechanics;
using UnityEngine;

namespace Player.States
{
public class Walking: GroundedBase
{
    private MovementStats _stats;
    private float _edgeLockedTime = 0f;

    public Walking(PlayerController p) : base(p) {
        _stats = P.movementStats;
    }

    public override void Enter() {
        base.Enter();
        P.animator.SetTrigger("Walk");
    }

    public override void FixedTick() {
        var intent = P.Intent;
        if (Mathf.Approximately(intent.Move.x, 0)) return;

        if (WillFall() && Time.time >= _edgeLockedTime + _stats.edgeRelockTime)
            P.LockMovement(_stats.onEdgeMovementLockTime);

        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        if (intent.SprintHeld)
            P.animator.SetBool("Sprinting", true);
        else
            P.animator.SetBool("Sprinting", false);

        var targetSpeed = horizontalInput *
                          _stats.walkSpeed *
                          (intent.SprintHeld ? _stats.sprintMultiplier : 1f) *
                          _stats.WalkSpeedMultiplier;

        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? _stats.walkAccel : _stats.walkDecel;

        P.CelerateX(targetSpeed, acceleration);
    }

    // Make it harder to fall off edges
    public bool WillFall() {
        Vector2 pos = P.transform.position;
        Vector2 positionAhead = pos + P.FacingDirection * 0.1f * Vector2.right;

        RaycastHit2D groundHit = Physics2D.Raycast(positionAhead,
            Vector2.down,
            P.GetSizeY() / 2 + 0.2f,
            P.terrainLayer);


        return groundHit.collider is null;
    }

}
}