using System;
using Player;
using Mechanics;
using UnityEngine;

namespace Enemies.States
{
public class Patrolling: EnemyState
{
    public Patrolling(BaseEnemy enemy) : base(enemy) { }

    public event Action<PlayerController> PlayerDetected;

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

        PlayerController pc = null;
        if (playerHit.collider != null)
            playerHit.collider.TryGetComponent<PlayerController>(out pc);

        PlayerDetected?.Invoke(pc);
    }
}
}