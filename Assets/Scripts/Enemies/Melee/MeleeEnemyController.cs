using Enemies.States;

namespace Enemies.Melee
{
public class MeleeEnemyController: BaseEnemy
{
    // states
    private Patrolling _patrollingState;

    protected override void Start() {
        base.Start();
        _patrollingState = new Patrolling(this);
        StateMachine.Initialize(_patrollingState);

    }
}
}