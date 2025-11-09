using Enemies.Golubok.States;
using Mechanics;

namespace Enemies.Golubok
{
public class GolubokController: BaseEnemy
{
    private Patrolling _patrolling;
    private Launching _launching;
    private bool _shouldLaunch = false;

    protected override void Start() {
        base.Start();
        _patrolling = new Patrolling(this);
        _launching = new Launching(this);

        _patrolling.PlayerDetected += () => { _shouldLaunch = true; };
        StateMachine.Initialize(_patrolling);

        StateMachine.AddTransition(_patrolling, _launching, ShouldLaunch);
        this.SetGravityScale(0); // The guy flies, so ye, kinda obvious
    }

    private bool ShouldLaunch() {
        if (_shouldLaunch) {
            _shouldLaunch = false;
            return true;
        }

        return false;
    }
}
}