using UnityEngine;

namespace Enemies.Stats
{
[System.Serializable]
public class CombatStats
{
    public int maxHealthPoints = 30;
    public int attackDamage = 10;
    public float attackRate = 1f;
    public float attackKnockback = 2f;
    [Header("Targeting")]
    public float noticeTime = 0.5f;
    public float detectionRange = 2f;
    public float attackRange = 1f;
    [Header("Melee specific")]
    public float attackDelay = 0.1f; // Delay from animation trigger to actual attack
    public float attackWidth = 0.2f;
    public float verticalKnockback = 1f;
}
}