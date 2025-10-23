using UnityEngine;

namespace Enemies.Stats
{
[System.Serializable]
public class CombatStats
{
    public int maxHealthPoints = 30;
    public int attackDamage = 10;
    public float attackRate = 2f;
    public float attackKnockback = 2f;
    public float attackDelay = 0.1f; // Delay from animation trigger to actual attack
    public float attackRange = 1f;
    public float attackHeight = 0.2f;
    public float attackWidth = 0.5f;
}
}