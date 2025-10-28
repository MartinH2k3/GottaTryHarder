using Enemies.States;
using Mechanics;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Pursuit: EnemyState
{
    public Pursuit(BaseEnemy enemy) : base(enemy) { }

    private float _timeout;

    public override void Enter() {
        base.Enter();
        // On enter, wait a bit before chasing the player
        _timeout = Time.time + E.combatStats.noticeTime;
    }

    public override void FixedTick() {
        base.FixedTick();

        if (Time.time < _timeout)
            return;

        int direction = E.TargetPos.x > E.Pos.x ? 1 : -1;

        if (E.CanWalkForward()) E.TurnAround();

        E.AccelerateX(direction * E.movementStats.chaseSpeed, E.movementStats.acceleration);

    }
}
}