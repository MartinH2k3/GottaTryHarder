using Enemies.States;
using Mechanics;
using Player;
using UnityEngine;
using Utils;

namespace Enemies.Boss.States
{
public class Bubbling: EnemyState<Boss>
{
    public Bubbling(Boss enemy) : base(enemy) {}

    private bool _stopGrowing;

    public override void Enter() {
        base.Enter();
        E.animator.Play("RobbyBubble");
        E.EnableBubbleCollider();
        _stopGrowing = false;
    }

    public override void FixedTick() {
        base.FixedTick();
        // Grow E.Transform scale over time to a max size using lerp
        if (!_stopGrowing) {
            float maxScale = 2.0f;
            float growSpeed = 0.5f;
            E.transform.localScale = Vector3.Lerp(E.transform.localScale, new Vector3(maxScale, maxScale, maxScale),
                growSpeed * Time.fixedDeltaTime);
            if (E.transform.localScale.x >= maxScale * 0.95f) {
                _stopGrowing = true;
            }
        }
    }

    public override void Exit() {
        base.Exit();
        E.DisableBubbleCollider();
    }

    public void HandleTriggerEnter(Collider2D other) {
        bool hitPlayer = Helpers.LayerInLayerMask(other.gameObject.layer, E.playerLayer);
        if (hitPlayer) {
            var player = other.gameObject.GetComponent<Player.PlayerController>();
            HitPlayer(player);
        }
    }

    public void HandleCollisionEnter(Collision2D other) {
        bool hitPlayer = Helpers.LayerInLayerMask(other.gameObject.layer, E.playerLayer);
        if (hitPlayer) {
            var player = other.gameObject.GetComponent<Player.PlayerController>();
            HitPlayer(player);
        }

        // if collision is not from the bottom, stop growing to avoid clipping through walls
        ContactPoint2D contact = other.contacts[0];
        if (Vector2.Dot(contact.normal, Vector2.up) < 0.5f) {
            _stopGrowing = true;
        }
    }

    private void HitPlayer(PlayerController player) {
        int knockbackDir = player.transform.position.x - E.transform.position.x > 0 ? 1 : -1;

        float stunDuration = Helpers.StunDurationEased(E.combatStats.knockbackStunDuration, E.combatStats.dashKnockbackStrength);
        player.LockMovement(stunDuration);

        float dashKnockback = E.combatStats.dashKnockbackStrength * E.combatStats.attackKnockback;
        player.AddForce(knockbackDir*dashKnockback, 3, ForceMode2D.Impulse);
    }
}
}