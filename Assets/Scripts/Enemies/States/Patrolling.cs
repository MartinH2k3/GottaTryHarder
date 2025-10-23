using MyPhysics;
using UnityEngine;

namespace Enemies.States
{
public class Patrolling: EnemyState
{
    public Patrolling(BaseEnemy enemy) : base(enemy) { }

    public override void FixedTick() {
        Debug.Log("Ticking Patrolling State");
        base.FixedTick();
        if (CanWalkForward())
            E.SetVelocityX(E.FacingDirection * E.movementStats.movementSpeed);
        else
            E.TurnAround();
    }

    private bool CanWalkForward() {
        Vector2 positionAhead = E.Pos + E.FacingDirection * E.movementStats.lookaheadDistance * Vector2.right;

        RaycastHit2D groundHit = Physics2D.Raycast(positionAhead,
            Vector2.down,
            E.GetSizeX() / 2 + E.movementStats.groundDetectionDistance,
            E.terrainLayer);

        RaycastHit2D wallhit = Physics2D.Raycast(E.Pos,
            Vector2.right * E.FacingDirection,
            E.movementStats.lookaheadDistance,
            E.terrainLayer);

        return groundHit.collider is not null && wallhit.collider is null;
    }
}
}