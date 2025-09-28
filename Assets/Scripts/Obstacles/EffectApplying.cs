using Player;
using UnityEngine;

namespace Obstacles
{
public static class EffectApplying
{
    public static void DealDamage(this IDamagingObstacle damager, IDamageable damaged, int damage) {
        if (!damaged.IsVulnerable || damaged.IsDead) return;
        damaged.HealthPoints -= damage;
        if (damaged.HealthPoints <= 0) {
            damaged.Die();
        }
    }

    public static void TakeDamage(this IDamageable damaged, int damage) {
        if (!damaged.IsVulnerable || damaged.IsDead) return;
        damaged.HealthPoints -= damage;
        if (damaged.HealthPoints <= 0) {
            damaged.Die();
        }
    }

    public static void Heal(this IDamageable healed, int healAmount) {
        healed.HealthPoints += healAmount;
        healed.HealthPoints = Mathf.Max(healed.HealthPoints, 0);
    }
}
}

