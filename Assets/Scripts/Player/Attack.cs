using UnityEngine;

namespace Player
{
public class Attack {
    [Header("Attack settings")]
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float attackRate = 2f; // attacks per second
    private float _nextAttackTime = 0f;


    [Header("Attack combos")]
    private float _lastAttackTime = float.NegativeInfinity;
    [SerializeField] private float jumpKickTime = 0.2f; // time window after jumping to do a jump kick
    [SerializeField] private float jumpKickDamageMultiplier = 1.5f; // attack damage * this
    [SerializeField] private float attackChainTime = 0.3f; // time window after attack to do another one


}

public enum AttackType {
    LeftJab,
    RightCross,
    Uppercut,
    JumpKick
}
}