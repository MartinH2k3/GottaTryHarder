using Enemies.Golubok.States;
using Mechanics;
using UnityEngine;

namespace Enemies.Golubok
{
public class GolubokController: BaseEnemy
{
    private Patrolling _patrolling;
    private Launching _launching;
    private bool _shouldLaunch = false;
    private bool _shouldStopLaunch = false;

    protected override void Start() {
        base.Start();
        _patrolling = new Patrolling(this);
        _launching = new Launching(this);

        _patrolling.PlayerDetected += () => { _shouldLaunch = true; };
        _launching.LaunchFinished += () => { _shouldStopLaunch = true; };

        StateMachine.Initialize(_patrolling);

        StateMachine.AddTransition(_patrolling, _launching, ShouldLaunch);
        StateMachine.AddTransition(_launching, _patrolling, ShouldStopLaunch);

        this.SetGravityScale(0); // The guy flies, so ye, kinda obvious
    }

    private bool ShouldLaunch() {
        if (_shouldLaunch) {
            _shouldLaunch = false;
            return true;
        }

        return false;
    }

    private bool ShouldStopLaunch() {
        if (_shouldStopLaunch) {
            _shouldStopLaunch = false;
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (StateMachine.Current == _launching)
            _launching.HandleCollision(collision);
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