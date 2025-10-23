﻿using Enemies.Melee.States;
using Enemies.States;
using UnityEngine;

namespace Enemies.Melee
{
public class MeleeEnemyController: BaseEnemy
{
    // states
    private Patrolling _patrolling;
    private Pursuit _pursuit;
    private Attacking _attacking;

    protected override void Start() {
        base.Start();
        _patrolling = new Patrolling(this);
        _pursuit = new Pursuit(this);
        _attacking = new Attacking(this);

        StateMachine.Initialize(_patrolling);

        _patrolling.PlayerDetected += (playerTransform) => {
            Target = playerTransform;
        };

        StateMachine.AddStartTransition(_patrolling, () => !TargetInRange());
        StateMachine.AddStartTransition(_pursuit, TargetInRange);

        StateMachine.AddTransition(_patrolling, _pursuit, TargetInRange);
        StateMachine.AddTransition(_pursuit, _patrolling, () => !TargetInRange(), 1);

        StateMachine.AddTransition(_pursuit, _attacking, TargetInAttackRange);
        StateMachine.AddExitTransition(_attacking, () => _attacking.IsAttackFinished);
    }

    /// <summary>
    /// Checks if the target is within attack range. Overrides the base method as attack are possible only horizontally.
    /// </summary>
    /// <returns></returns>
    protected override bool TargetInAttackRange() {
        if (Target is null)
            return false;

        return Mathf.Abs(Target.position.x - Pos.x) < combatStats.attackRange
            ;//&& Mathf.Abs(Target.position.y - Pos.y) < EnemyHeight/2;
    }
}
}