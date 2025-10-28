using Enemies.States;
using Mechanics;
using UnityEngine;

namespace Enemies.Golubok.States
{
public class Patrolling: EnemyState
{
    private float _oscillationStartTime;

    public Patrolling(BaseEnemy enemy) : base(enemy) {}

    public override void Enter() {
        base.Enter();
        _oscillationStartTime = Time.time - E.movementStats.oscillationPeriod/4; // Start in middle of half-oscillation
    }

    public override void FixedTick() {
        base.FixedTick();
        Oscillate();
    }

    private void Oscillate() {
        var omega = 2 * Mathf.PI / E.movementStats.oscillationPeriod;
        var elapsed = Time.time - _oscillationStartTime;
        var targetSpeed = E.movementStats.oscillationAmplitude * Mathf.Sin(omega * elapsed);

        E.SetVelocityY(targetSpeed);
    }

}
}