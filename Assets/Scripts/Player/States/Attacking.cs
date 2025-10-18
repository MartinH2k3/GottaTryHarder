using UnityEngine;

namespace Player.States
{
public class Attacking: PlayerState
{
    public bool IsAttackFinished => P.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    public Attacking(PlayerController p): base(p) { }

    public override void Enter() {
        P.animator.SetTrigger("Attack");
    }
}

public enum AttackType {
    LeftJab,
    RightCross,
    Uppercut,
    JumpKick
}
}