using Enemies.States;
using Managers;
using Mechanics;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Attacking: EnemyState
{
    public Attacking(BaseEnemy enemy) : base(enemy) { }

    public bool IsAttackFinished;
    private bool AttackAnimationPlaying => E.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
    private bool AttackAnimFinished => E.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    private bool AttackApplyTime => E.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=  E.animationStats.damageApplyNormalizedTime;
    private bool _attackExecuted;

    public override void Enter() {
        base.Enter();
        // Stop movement from pursuit
        E.SetVelocityX(0);

        AudioManager.Instance.PlaySFX(E.sounds.attack);
        E.animator.SetBool("Attacking", true);
        IsAttackFinished = false;
        _attackExecuted = false;
    }

    public override void Tick() {
        base.Tick();
        if (AttackAnimationPlaying && AttackApplyTime && !_attackExecuted) {
            Attack();
            _attackExecuted = true;
        }
        if (AttackAnimFinished && _attackExecuted) {
            // Using this instead the controller using AttackAnimFinished directly, to avoid issues if AttackApplyTime is at the end of the animation
            IsAttackFinished = true;
        }
    }

    private void Attack() {

        var cornerA = (Vector2)E.transform.position +
                      new Vector2(0, -E.combatStats.attackWidth);
        var cornerB = (Vector2)E.transform.position +
                      new Vector2(E.combatStats.attackRange * E.FacingDirection,
                          E.combatStats.attackWidth);
        // Compute the axis-aligned rectangle from the two opposite corners and draw it for a short duration
        var min = new Vector2(Mathf.Min(cornerA.x, cornerB.x), Mathf.Min(cornerA.y, cornerB.y));
        var max = new Vector2(Mathf.Max(cornerA.x, cornerB.x), Mathf.Max(cornerA.y, cornerB.y));

        // if rectangle(min, max) overlaps with player collider (E.Target's collider), deal damage to player
        var player = E.Target;
        var playerCol = player.GetComponent<Collider2D>();
        bool playerWithinHitbox = playerCol.bounds.min.x <= max.x &&
                              playerCol.bounds.max.x >= min.x &&
                              playerCol.bounds.min.y <= max.y &&
                              playerCol.bounds.max.y >= min.y;
        if (playerWithinHitbox) {
            player.TakeDamage(E.combatStats.attackDamage);
            player.SetVelocityX(0); // So that player can't resist initial knockback by moving
            player.AddForce(new Vector2(
                E.combatStats.attackKnockback * E.FacingDirection,
                E.combatStats.verticalKnockback),
                ForceMode2D.Impulse);
            Debug.Log("Velocity after" + player.GetVelocity());
            // Need to stun for enough time so that he doesn't immediately cancel the stun with movement
            float stunDuration = 0.1f * E.combatStats.attackKnockback;
            player.LockMovement(stunDuration);
        }

        DisplayAttackHitbox(min, max);
    }

    public override void Exit() {
        base.Exit();
        E.animator.SetBool("Attacking", false);
        E.LastAttackTime = Time.time;
    }

    private void DisplayAttackHitbox(Vector2 min, Vector2 max) {
        Vector3 bl = new Vector3(min.x, min.y, 0f);
        Vector3 tl = new Vector3(min.x, max.y, 0f);
        Vector3 tr = new Vector3(max.x, max.y, 0f);
        Vector3 br = new Vector3(max.x, min.y, 0f);

        const float drawDuration = 1.15f; // seconds
        var color = Color.red;
        Debug.DrawLine(bl, tl, color, drawDuration);
        Debug.DrawLine(tl, tr, color, drawDuration);
        Debug.DrawLine(tr, br, color, drawDuration);
        Debug.DrawLine(br, bl, color, drawDuration);
    }
}
}


