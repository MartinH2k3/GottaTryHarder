using System;
using Enemies.States;
using Player;
using Mechanics;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Patrolling: EnemyState<MeleeEnemy>
{

    public Patrolling(MeleeEnemy enemy) : base(enemy) {}

    public event Action<PlayerController> PlayerDetected;

    public override void Enter() {
        base.Enter();
        E.animator.SetBool("Walking", true);
    }

    public override void Exit() {
        base.Exit();
        E.SetVelocityX(0f);
    }

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
            playerHit.collider.TryGetComponent(out pc);

        PlayerDetected?.Invoke(pc);
    }
}
}