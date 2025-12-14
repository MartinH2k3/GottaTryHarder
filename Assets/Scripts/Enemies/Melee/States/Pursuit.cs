using Enemies.States;
using Mechanics;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Pursuit: EnemyState<MeleeEnemy>
{
    public Pursuit(MeleeEnemy enemy) : base(enemy) { }

    private float _timeout;
    private bool _waiting;

    public override void Enter() {
        base.Enter();
        // On enter, wait a bit before chasing the player
        E.animator.SetBool("Walking", false);
        _timeout = Time.time + E.combatStats.noticeTime;
        _waiting = true;
    }

    public override void FixedTick() {
        base.FixedTick();

        if (Time.time < _timeout)
            return;
        if (_waiting) {
            E.animator.SetBool("Walking", true);
            _waiting = false;
        }

        int direction = E.TargetPos.x > E.Pos.x ? 1 : -1;

        if (E.CanWalkForward()) E.TurnAround();

        E.AccelerateX(direction * E.movementStats.chaseSpeed, E.movementStats.acceleration);

    }
}
}