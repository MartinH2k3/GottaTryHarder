using Infrastructure.StateMachine;

namespace Enemies.States
{
// Non-generic version for convenience
public class EnemyState : EnemyState<BaseEnemy>
{
    protected EnemyState(BaseEnemy enemy) : base(enemy) { }
}

public class EnemyState<TEnemy> : EmptyState where TEnemy : BaseEnemy
{
    protected readonly TEnemy E;

    protected EnemyState(TEnemy enemy) => E = enemy;
}
}