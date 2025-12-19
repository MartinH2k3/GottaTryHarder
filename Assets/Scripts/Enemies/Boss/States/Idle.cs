using Enemies.States;
using Mechanics;
using UnityEngine;
using Utils;

namespace Enemies.Boss.States
{
public class Idle: BossState
{
    public Idle(Boss enemy) : base(enemy) {}

    private float _endTime;
    private bool _exitEventInvoked;

    public override void Enter() {
        base.Enter();
        E.animator.Play("Idle");
        _endTime = Time.time + E.GetBreakDuration();
        _exitEventInvoked = false;
    }

    public override void FixedTick() {
        base.FixedTick();
        E.SetVelocityX(0);
        if (Time.time >= _endTime && !_exitEventInvoked) {
            _exitEventInvoked = true;
            ShouldExit?.Invoke();
        }
    }
    public void HandleCollisionEnter(Collision2D other) {
        bool hitPlayer = Helpers.LayerInLayerMask(other.gameObject.layer, E.playerLayer);
        if (!hitPlayer)
            return;

        // Don't get pushed by the player
        E.SetVelocity(Vector2.zero);

        var player = other.gameObject.GetComponent<Player.PlayerController>();
        int knockbackDir = player.transform.position.x - E.transform.position.x > 0 ? 1 : -1;

        float stunDuration = Helpers.StunDurationEased(E.combatStats.knockbackStunDuration, E.combatStats.baseContactKnockbackStrength);
        player.LockMovement(stunDuration);

        float idleKnockback = E.combatStats.baseContactKnockbackStrength * E.combatStats.attackKnockback;
        player.AddForce(knockbackDir*idleKnockback, 3, ForceMode2D.Impulse);
    }
}
}