using Enemies.Golubok.States;
using Mechanics;
using UnityEngine;
using Utils;

namespace Enemies.Golubok
{
public class Golubok: BaseEnemy
{
    private Patrolling _patrolling;
    private Launching _launching;
    private Shooting _shooting;
    private bool _shouldLaunch = false;
    private bool _shouldStopLaunch = false;

    [SerializeField] private GameObject projectilePrefab;
    public GameObjectPool MissilePool { get; private set; }

    protected override void Start() {
        base.Start();
        _patrolling = new Patrolling(this);
        _launching = new Launching(this);
        _shooting = new Shooting(this);

        MissilePool = GameObjectPool.GetPool(projectilePrefab, transform, 2);

        _patrolling.PlayerDetected += () => { _shouldLaunch = true; };
        _launching.LaunchFinished += () => { _shouldStopLaunch = true; };

        StateMachine.Initialize(_patrolling);

        StateMachine.AddTransition(_patrolling, _launching, ShouldLaunch);
        StateMachine.AddTransition(_launching, _shooting, ShouldStopLaunch);
        StateMachine.AddTransition(_shooting, _patrolling, () => _shooting.AttackAnimFinished);
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

    public void OnShootChargeComplete() {
        if (StateMachine.Current == _shooting) {
            _shooting.Shoot();
        }
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