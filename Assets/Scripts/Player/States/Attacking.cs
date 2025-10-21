using Enemies;
using MyPhysics;
using Player.Stats;
using UnityEngine;

namespace Player.States
{
public class Attacking: PlayerState
{
    public bool IsAttackFinished => P.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    private AttackStats _stats;

    public Attacking(PlayerController p) : base(p) {
        _stats = P.attackStats;
    }

    public override void Enter() {
        P.animator.SetTrigger("Attack");
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
    }

    private void Attack(AttackType attackType) {
        var bottomLeft = (Vector2)P.transform.position + new Vector2(0, -_stats.attackHeight);
        var topRight = (Vector2)P.transform.position + new Vector2(_stats.attackRange * P.FacingDirection, _stats.attackHeight);

        Collider2D[] hits = Physics2D.OverlapAreaAll(bottomLeft, topRight, P.attackableLayer);

        foreach (var collider in hits) {
            var attackable = collider.GetComponent<IAttackable>();
            if (attackable is not null) {
                attackable.TakeDamage(_stats.attackDamage);
            }
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