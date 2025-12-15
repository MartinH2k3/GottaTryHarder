using Enemies.States;
using Mechanics;
using UnityEngine;
using Utils;

namespace Enemies.Boss.States
{
public class Idle: EnemyState<Boss>
{
    public Idle(Boss enemy) : base(enemy) {}

    public override void Enter() {
        base.Enter();
        E.animator.Play("Idle");
    }

    public void HandleCollisionEnter(Collision2D other) {
        bool hitPlayer = Helpers.LayerInLayerMask(other.gameObject.layer, E.playerLayer);
        if (!hitPlayer)
            return;

        // Don't get pushed by the player
        E.SetVelocity(Vector2.zero);

        var player = other.gameObject.GetComponent<Player.PlayerController>();
        int knockbackDir = player.transform.position.x - E.transform.position.x > 0 ? 1 : -1;

        float stunDuration = Helpers.StunDurationEased(E.combatStats.knockbackStunDuration, E.combatStats.idleKnockbackStrength);
        player.LockMovement(stunDuration);

        float idleKnockback = E.combatStats.idleKnockbackStrength * E.combatStats.attackKnockback;
        player.AddForce(knockbackDir*idleKnockback, 3, ForceMode2D.Impulse);
    }
}
}