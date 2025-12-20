using Enemies.States;
using Managers;
using Mechanics;
using UnityEngine;

namespace Enemies.Golubok.States
{
public class Launching: EnemyState<Golubok>
{
    public Launching(Golubok enemy) : base(enemy) {}

    private float _launchTime;
    private bool _launched;
    private float _launchEndTime;
    public event System.Action LaunchFinished;
    private bool _finishEventInvoked;

    public override void Enter() {
        base.Enter();
        E.SetVelocity(0,0);
        _launchTime = Time.time + E.combatStats.launchChargeTime;
        _launchEndTime = _launchTime + E.combatStats.launchDuration;
        _launched = false;
        _finishEventInvoked = false;

        bool playerOnLeft = E.TargetPos.x < E.Pos.x;
        bool facingLeft = E.FacingDirection > 0; // Default sprite orientation faces left for Golubok
        if (playerOnLeft != facingLeft)
            E.TurnAround();

        E.animator.Play("Charge Dash");
    }

    public override void Exit() {
        base.Exit();
        E.SetVelocity(Vector2.zero);
        E.transform.rotation = Quaternion.identity;
    }

    public override void FixedTick() {
        base.FixedTick();
        if (Time.time > _launchTime && !_launched) {
            E.animator.Play("Dash");
            Launch();
            _launched = true;
        }
        if (Time.time > _launchEndTime && !_finishEventInvoked) {
            LaunchFinished?.Invoke();
            _finishEventInvoked = true;
        }
    }

    private void Launch() {
        _launched = true;
        E.SetVelocity(0,0);
        var dir = E.TargetPos - E.Pos;
        Rotate(dir);
        E.AddForce(dir.normalized * E.combatStats.launchForce, ForceMode2D.Impulse);

        AudioManager.Instance.PlaySFX(E.sounds.attack);
    }

    public void HandleCollision(Collision2D collision)
    {
        E.SetVelocity(0, 0);

        if (!_launched) // Ignore collisions before launch
            return;

        bool hitPlayer = ((1 << collision.gameObject.layer) & E.playerLayer) != 0;
        bool hitTerrain = ((1 << collision.gameObject.layer) & E.terrainLayer) != 0;

        if ( hitPlayer | hitTerrain ) {
            var contact = collision.GetContact(0);
            Vector2 normal = contact.normal;

            Vector2 impactVelocity = -collision.relativeVelocity; // relativeVelocity is calculated from the terrain/player perspective

            float speed = impactVelocity.magnitude;

            Vector2 bounceDirection = Vector2.Reflect(impactVelocity.normalized, normal);

            if (hitPlayer) {
                // Always bounce upwards when hitting the player. Bouncing downwards makes poop attack useless
                E.Target.TakeDamage(E.combatStats.attackDamage);
                bounceDirection = PullTowardAngleEased(bounceDirection, E.combatStats.bounceOffAngle);
            }


            Vector2 bounceForce = bounceDirection * speed * E.combatStats.bounceFactor;

            Rotate(bounceDirection);
            E.AddForce(bounceForce, ForceMode2D.Impulse);
        }
    }

    private Vector2 PullTowardAngleEased(Vector2 dir, float targetAngleDeg, float maxPull01 = 0.85f)
    {
        dir.Normalize();

        // Angle from the x-axis in both directions is good
        float mirroredAngleDeg = 180f - targetAngleDeg;

        float current = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float deltaRight = Mathf.DeltaAngle(current, targetAngleDeg);
        float deltaLeft = Mathf.DeltaAngle(current, mirroredAngleDeg);

        float chosenDelta = Mathf.Abs(deltaRight) <= Mathf.Abs(deltaLeft) ? deltaRight : deltaLeft;

        float absDelta = Mathf.Abs(chosenDelta);

        // Ease: weak pull when close, strong pull when far
        float t = Mathf.InverseLerp(0f, 30f, absDelta);
        t *= t; // quadratic ease

        float newAngle = current + chosenDelta * (t * maxPull01);

        float rad = newAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

    }


    public void Rotate(Vector2 direction) {
        if (direction.sqrMagnitude < 0.01f) return;

        bool movingLeft = direction.x < 0;
        bool facingLeft = E.FacingDirection > 0;
        if (movingLeft != facingLeft)
            E.TurnAround();

        float angle = Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
        angle *= E.FacingDirection * -1; // * -1 because I want positive rotation when going downwards

        E.transform.rotation = Quaternion.Euler(0, 0, angle);

    }
}
}