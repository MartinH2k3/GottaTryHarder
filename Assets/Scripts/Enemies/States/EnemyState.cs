using Infrastructure.StateMachine;

namespace Enemies.States
{
public class EnemyState: EmptyState
{
    protected readonly BaseEnemy E;

    protected EnemyState(BaseEnemy enemy) => E = enemy;
}
}