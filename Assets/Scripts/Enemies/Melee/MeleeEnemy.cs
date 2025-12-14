using Enemies.Melee.States;
using Mechanics;
using UnityEngine;

namespace Enemies.Melee
{
public class MeleeEnemy: BaseEnemy
{
    // states
    private Patrolling _patrolling;
    private Pursuit _pursuit;
    private Attacking _attacking;

    protected override void Awake() {
        base.Awake();
        animator.SetFloat("Attack Speed Multiplier", combatStats.attackSpeedMult);
    }

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
        StateMachine.AddStartTransition(_attacking, TargetInAttackRange);

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
        return Mathf.Abs(TargetPos.x - Pos.x) < combatStats.attackRange
            ;//&& Mathf.Abs(Target.position.y - Pos.y) < EnemyHeight/2;
    }

    /// <summary>
    /// Checks if there is a ground to walk on ahead. Also checks for walls in the way.
    /// </summary>
    public bool CanWalkForward() {
        Vector2 positionAhead = Pos + FacingDirection * movementStats.lookaheadDistance * Vector2.right;

        RaycastHit2D groundHit = Physics2D.Raycast(positionAhead,
            Vector2.down,
            this.GetSizeY() / 2 + movementStats.groundDetectionDistance,
            terrainLayer);

        RaycastHit2D wallhit = Physics2D.Raycast(Pos,
            Vector2.right * FacingDirection,
            movementStats.lookaheadDistance,
            terrainLayer);

        // Visualize ground check ray (yellow)
        Debug.DrawRay(positionAhead,
            Vector2.down * (this.GetSizeY() / 2 + movementStats.groundDetectionDistance),
            Color.yellow);

        // Visualize wall check ray (red)
        Debug.DrawRay(Pos,
            FacingDirection * movementStats.lookaheadDistance * Vector2.right,
            Color.red);


        return groundHit.collider is not null && wallhit.collider is null;
    }
}
}