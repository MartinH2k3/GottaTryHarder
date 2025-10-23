using System;
using MyPhysics;
using UnityEngine;

namespace Enemies.States
{
public class Patrolling: EnemyState
{
    public Patrolling(BaseEnemy enemy) : base(enemy) { }

    public event Action<Transform> PlayerDetected;

    public override void FixedTick() {
        base.FixedTick();
        if (E.CanWalkForward())
            E.SetVelocityX(E.FacingDirection * E.movementStats.walkSpeed);
        else
            E.TurnAround();
        SeekPlayer();
    }



    private void SeekPlayer() {
        RaycastHit2D playerHit = Physics2D.Raycast(E.Pos,
            Vector2.right * E.FacingDirection,
            E.combatStats.detectionRange,
            E.playerLayer);
         PlayerDetected?.Invoke(playerHit.collider?.transform);
    }
}
}