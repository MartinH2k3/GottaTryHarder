using UnityEngine;

namespace Player.Stats
{
[System.Serializable]
public class CombatStats
{
    public int maxHealthPoints = 100;
    [Tooltip("Seconds of invulnerability after being hit.")]
    public float invulnerabilityDuration = 1f;

    [Tooltip("Time window after attack to do another one.")]
    public float attackComboTime = 1f;
    [Tooltip("Time window after jumping to do a jump kick.")]
    public float jumpKickTime = 0.25f;
    [Tooltip("Damage multiplier for jump kick attack.")]
    public float jumpKickDamageMultiplier = 1.25f;

    public int attackDamage = 10;
    public float attackKnockback = 2f;

    public float attackRange = 1f;
    [Tooltip("Height of the center of the attack hitbox.")]
    public float attackHeight = 0.2f;
    [Tooltip("Width of the attack hitbox.")]
    public float attackWidth = 0.5f;

    [Tooltip("Attacks per second.")]
    public float attackRate = 2f;
    [Tooltip("Delay from animation trigger to actual attack.")]
    public float attackDelay = 0.1f;

    [Tooltip("Health regained after hitting an enemy.")]
    public int lifeSteal = 0;

    public bool jumpKickUnlocked = false;
    public bool comboUnlocked = false;
}
}