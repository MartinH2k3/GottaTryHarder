using Enemies.Golubok.States;
using Mechanics;

namespace Enemies.Golubok
{
public class GolubokController: BaseEnemy
{
    private Patrolling _patrolling;

    protected override void Start() {
        base.Start();
        _patrolling = new Patrolling(this);
        StateMachine.Initialize(_patrolling);

        this.SetGravityScale(0); // The guy flies, so ye, kinda obvious
    }
}
}