using Enemies.States;
using Mechanics;
using UnityEngine;
using Utils;

namespace Enemies.Boss.States
{
public class Dashing: BossState
{
    public Dashing(Boss enemy) : base(enemy) {}

    private int _dashDirection;
    private bool _exitEventInvoked;
    private float _ogGravity;
    private float _dashEndTime;


    public override void Enter() {
        base.Enter();
        E.animator.Play("Leap");

        _dashDirection = E.Target.transform.position.x > E.Pos.x ? 1 : -1;
        if (_dashDirection != E.FacingDirection)
            E.TurnAround();

        _dashEndTime = Time.time + E.combatStats.dashDuration;
        _exitEventInvoked = false;

        _ogGravity = E.GetGravityScale();
        E.SetGravityScale(0f);
    }

    public override void FixedTick() {
        base.FixedTick();
        E.SetVelocity(_dashDirection * E.combatStats.dashSpeed, 0);

        if (Time.time >= _dashEndTime && !_exitEventInvoked) {
            _exitEventInvoked = true;
            ShouldExit?.Invoke();
        }
    }

    public override void Exit() {
        base.Exit();
        E.SetGravityScale(_ogGravity);
        E.SetVelocity(Vector2.zero);
    }

    public void HandleCollisionEnter(Collision2D other) {
        bool hitPlayer = Helpers.LayerInLayerMask(other.gameObject.layer, E.playerLayer);
        if (!hitPlayer)
            return;

        // Don't get pushed by the player
        E.SetVelocity(Vector2.zero);

        var player = other.gameObject.GetComponent<Player.PlayerController>();
        int knockbackDir = player.transform.position.x - E.transform.position.x > 0 ? 1 : -1;

        float stunDuration = Helpers.StunDurationEased(E.combatStats.knockbackStunDuration, E.combatStats.dashKnockbackStrength);
        player.LockMovement(stunDuration);

        float dashKnockback = E.combatStats.dashKnockbackStrength * E.combatStats.attackKnockback;
        player.AddForce(knockbackDir*dashKnockback, 3, ForceMode2D.Impulse);
    }
}
}