using Managers;
using Mechanics;
using UnityEngine;
using Utils;

namespace Enemies.Boss.States
{
public class Bubbling: BossState
{
    public Bubbling(Boss enemy) : base(enemy) {}

    private bool _stopGrowing;
    private float AnimationDuration => E.animator.GetCurrentAnimatorStateInfo(0).length;
    private bool AnimationCompleted => E.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    private bool _stopEventInvoked;

    public override void Enter() {
        base.Enter();
        E.animator.Play("RobbyBubble");
        AudioManager.Instance.PlaySFX(E.sounds.attack);
        E.EnableBubbleCollider();
        _stopGrowing = false;
        _stopEventInvoked = false;
    }

    public override void FixedTick() {
        base.FixedTick();
        if (!_stopGrowing) {
            float maxScale = E.combatStats.maxBubbleSize * E.transform.localScale.x; // scale according to facing direction
            E.transform.localScale = Vector3.Lerp(E.transform.localScale, new Vector3(maxScale, maxScale, 1), Time.fixedDeltaTime / AnimationDuration);
            if (E.transform.localScale.x >= maxScale * 0.95f) {
                _stopGrowing = true;
            }
        }
        if (AnimationCompleted && !_stopEventInvoked) {
            ShouldExit?.Invoke();
            _stopEventInvoked = true;
        }
    }

    public override void Exit() {
        base.Exit();
        E.DisableBubbleCollider();
        E.transform.localScale = Vector3.one;
    }

    public void HandleTriggerEnter(Collider2D other) {
        bool hitPlayer = Helpers.LayerInLayerMask(other.gameObject.layer, E.playerLayer);
        if (hitPlayer) {
            HitPlayer();
        }
    }

    public void HandleCollisionEnter(Collision2D other) {
        bool hitPlayer = Helpers.LayerInLayerMask(other.gameObject.layer, E.playerLayer);
        if (hitPlayer) {
            HitPlayer();
        }

        // if collision is not from the bottom, stop growing to avoid clipping through walls
        ContactPoint2D contact = other.contacts[0];
        if (Vector2.Dot(contact.normal, Vector2.up) < 0.5f) {
            _stopGrowing = true;
        }
    }

    private void HitPlayer() {
        int knockbackDir = E.TargetPos.x - E.Pos.x > 0 ? 1 : -1;

        E.Target.TakeDamage(E.combatStats.attackDamage);

        float stunDuration = Helpers.StunDurationEased(E.combatStats.knockbackStunDuration, E.combatStats.dashKnockbackStrength);
        E.Target.LockMovement(stunDuration);

        float dashKnockback = E.combatStats.dashKnockbackStrength * E.combatStats.attackKnockback;
        E.Target.AddForce(knockbackDir*dashKnockback, 3, ForceMode2D.Impulse);
    }
}
}