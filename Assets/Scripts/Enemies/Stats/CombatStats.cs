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
    public float noticeTime = 0.5f; // Time for the enemy to process in their head "Oh wait, there is a guy, I'm going after him"
    public float detectionRange = 2f;
    public float attackRange = 1f;
    [Tooltip("How often to run the algorithm.")]
    public float detectionPeriod = 0.2f;
    [Header("Melee specific")]
    public float attackDelay = 0.1f; // Delay from animation trigger to actual attack
    public float attackWidth = 0.2f;
    public float verticalKnockback = 1f;
    [Header("Golubok specific")]
    public float chargeTime = 1f;
    public float launchForce = 10f;
}
}