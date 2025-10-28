using Enemies;
using Mechanics;
using Player.Stats;
using UnityEngine;

namespace Player.States
{
public class Attacking: PlayerState
{
    public bool IsAttackFinished => P.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
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
        P.animator.SetTrigger("Attack");

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
            var comboStep = P.ComboStep % 4;
            switch (comboStep) {
                case 0:
                    Attack(AttackType.LeftJab);
                    break;
                case 1:
                    Attack(AttackType.RightCross);
                    break;
                case 2:
                    Attack(AttackType.Uppercut);
                    break;
            }

            P.AdvanceCombo();
        }

        _attackExecuted = true;
    }

    private void Attack(AttackType attackType) {
        var bottomLeft = (Vector2)P.transform.position + new Vector2(0, -_stats.attackHeight);
        var topRight = (Vector2)P.transform.position + new Vector2(_stats.attackRange * P.FacingDirection, _stats.attackHeight);

        int count = Physics2D.OverlapArea(bottomLeft, topRight, _targetFilter, _hits);

        if (count > _hits.Length) {
            _hits = new Collider2D[count + _hits.Length];
            count = Physics2D.OverlapArea(bottomLeft, topRight, _targetFilter, _hits);
        }

        for (int i = 0; i < count; i++) {
            var collider = _hits[i];

            var attackable = collider.GetComponent<IAttackable>();
            if (attackable is null)
                continue;
            attackable.TakeDamage(_stats.attackDamage);

            // if physics object, apply force
            var physicsBody = collider.GetComponent<IPhysicsMovable>();
            if (physicsBody is not null) {
                physicsBody.AddForce(new Vector2(_stats.attackKnockback * P.FacingDirection, 2f), ForceMode2D.Impulse);
            }
        }
    }
}

public enum AttackType {
    LeftJab,
    RightCross,
    Uppercut,
    JumpKick
}
}