using Enemies.Melee.States;
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

    private bool CanAttack => LastAttackTime + (1f / combatStats.attackRate) < Time.time;

    protected override void Start() {
        base.Start();
        _patrolling = new Patrolling(this);
        _pursuit = new Pursuit(this);
        _attacking = new Attacking(this);

        StateMachine.Initialize(_patrolling);

        _patrolling.PlayerDetected += (player) => {
            Target = player;
        };

        StateMachine.AddStartTransition(_patrolling, () => !TargetInRange());
        StateMachine.AddStartTransition(_pursuit, TargetInRange);

        StateMachine.AddTransition(_patrolling, _pursuit, TargetInRange);
        StateMachine.AddTransition(_pursuit, _patrolling, () => !TargetInRange(), 1);

        StateMachine.AddTransition(_pursuit, _attacking, () => TargetInAttackRange() && CanAttack);
        StateMachine.AddExitTransition(_attacking, () => _attacking.IsAttackFinished);
    }

    /// <summary>
    /// Checks if the target is within attack range. Overrides the base method as attack are possible only horizontally.
    /// </summary>
    /// <returns></returns>
    protected override bool TargetInAttackRange() {
        return Mathf.Abs(TargetPos.x - Pos.x) < combatStats.attackRange
            ;//&& Mathf.Abs(Target.position.y - Pos.y) < EnemyHeight/2;
    }

    private readonly Vector3 _labelOffset = new (0, 1, 0);
    private void OnDrawGizmos()
    {
        var stateName = StateMachine?.Current?.GetType().Name ?? "None";
        var pos = transform.position + _labelOffset;

        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.Label(pos, stateName);
    }
}
}