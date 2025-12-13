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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (StateMachine.Current != _launching) return;

        if (((1 << collision.gameObject.layer) & (terrainLayer | playerLayer)) != 0)
        {
            Debug.Log($"Collision: {collision.gameObject.name}, Layer: {collision.gameObject.layer}, TerrainMask: {terrainLayer.value}, PlayerMask: {playerLayer.value}, CurrentState: {StateMachine.Current?.GetType().Name}");
            var contact = collision.GetContact(0);
            var normal = contact.normal;
            var impactSpeed = collision.relativeVelocity.magnitude;
            Debug.Log($"Impact Speed: {impactSpeed}, Normal: {normal}");
            _launching.Bounce(normal, impactSpeed, combatStats.bounceFactor);
        }
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