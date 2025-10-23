using Enemies.Melee.States;
using Enemies.States;

namespace Enemies.Melee
{
public class MeleeEnemyController: BaseEnemy
{
    // states
    private Patrolling _patrolling;
    private Pursuit _pursuit;

    protected override void Start() {
        base.Start();
        _patrolling = new Patrolling(this);
        _pursuit = new Pursuit(this);

        StateMachine.Initialize(_patrolling);

        _patrolling.PlayerDetected += (playerTransform) => {
            Target = playerTransform;
        };

        StateMachine.AddTransition(_patrolling, _pursuit, TargetInRange);
        StateMachine.AddTransition(_pursuit, _patrolling, () => !TargetInRange());
    }
}
}