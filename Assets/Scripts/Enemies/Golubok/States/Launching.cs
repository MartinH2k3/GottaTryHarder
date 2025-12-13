using Enemies.States;
using Mechanics;
using UnityEngine;

namespace Enemies.Golubok.States
{
public class Launching: EnemyState
{
    public Launching(BaseEnemy enemy) : base(enemy) {}

    private float launchTime;
    private bool launched;

    public override void Enter() {
        base.Enter();
        E.SetVelocity(0,0);
        launchTime = Time.time + E.combatStats.chargeTime;
        launched = false;
    }

    public override void FixedTick() {
        base.FixedTick();
        if (Time.time > launchTime && !launched)
            Launch();
    }

    private void Launch() {
        launched = true;
        E.SetVelocity(0,0);
        var dir = E.Pos - E.TargetPos;
        E.AddForce(dir.normalized * E.combatStats.launchForce, ForceMode2D.Impulse);
    }


    public void Bounce(Vector2 normal, float speed, float bounceFactor)
    {
        var reflected = Vector2.Reflect(E.GetVelocity().normalized, normal);
        Debug.Log($"Bounced direction: {reflected}, Normal: {normal}");
        E.SetVelocity(reflected * speed * bounceFactor);
    }
}
}