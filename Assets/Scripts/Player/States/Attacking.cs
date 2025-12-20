using Enemies;
using Managers;
using Mechanics;
using Player.Stats;
using UnityEngine;

namespace Player.States
{
public class Attacking: PlayerState
{
    public bool IsAttackFinished => P.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
                                    (P.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
                                     P.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") ||
                                     P.animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"));
    private CombatStats _stats;
    private float _attackStartTime;
    private bool _attackExecuted;

    private Collider2D[] _hits = new Collider2D[8];
    private ContactFilter2D _targetFilter = new ContactFilter2D();

    public Attacking(PlayerController p) : base(p) {
        _stats = P.combatStats;
        _targetFilter.layerMask = P.attackableLayer;
    }

    public override void Enter() {
        if (Time.time < P.LastAttackTime + P.combatStats.attackComboTime)
            P.AdvanceCombo();

        P.animator.SetInteger("ComboStep", P.ComboStep);
        P.animator.SetTrigger("Attack");
        AudioManager.Instance.PlaySFX(P.sounds.attack);

        _attackStartTime = Time.time + _stats.attackDelay;
        _attackExecuted = false;
    }

    public override void Tick() {
        if (Time.time < _attackStartTime || _attackExecuted) {
            return;
        }

        if (!_stats.comboUnlocked) {
            Attack(AttackType.LeftJab);
        }
        else if (P.LastJumpTime + _stats.jumpKickTime > Time.time && !P.IsGrounded && _stats.jumpKickUnlocked) {
            Attack(AttackType.JumpKick);
            P.ResetCombo();
        }
        else {
            var comboStep = P.ComboStep % 2;
            switch (comboStep) {
                case 0:
                    Attack(AttackType.LeftJab);
                    break;
                case 1:
                    Attack(AttackType.RightCross);
                    break;
            }

            P.AdvanceCombo();
        }

        _attackExecuted = true;
    }

    private void Attack(AttackType attackType) {
        var bottomLeft = (Vector2)P.transform.position + new Vector2(0, -_stats.attackHeight);
        var topRight = (Vector2)P.transform.position + new Vector2(_stats.attackRange * P.FacingDirection, _stats.attackHeight);

        DisplayAttackHitbox(bottomLeft, topRight);

        int count = Physics2D.OverlapArea(bottomLeft, topRight, _targetFilter, _hits);

        if (count > _hits.Length) {
            _hits = new Collider2D[count + _hits.Length];
            count = Physics2D.OverlapArea(bottomLeft, topRight, _targetFilter, _hits);
        }

        bool hitSomeone = false;
        for (int i = 0; i < count; i++) {
            var collider = _hits[i];
            // If somehow hitting self, skip
            if (collider.transform.root == P.transform.root)
                continue;

            var attackable = collider.GetComponent<IDamageable>();
            if (attackable is null)
                continue;
            attackable.TakeDamage(_stats.attackDamage);
            hitSomeone = true;

            // if physics object, apply force
            var physicsBody = collider.GetComponent<IPhysicsMovable>();
            if (physicsBody is not null) {
                physicsBody.AddForce(new Vector2(_stats.attackKnockback * P.FacingDirection, 2f), ForceMode2D.Impulse);
            }
        }
        if (hitSomeone && _stats.lifeSteal > 0) {
            P.Heal(_stats.lifeSteal);
        }
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

public enum AttackType {
    LeftJab,
    RightCross,
    JumpKick
}
}