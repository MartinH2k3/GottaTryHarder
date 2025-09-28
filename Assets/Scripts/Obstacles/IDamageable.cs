using UnityEngine;

namespace Obstacles
{
public interface IDamageable
{
    int HealthPoints { get; set; }
    bool IsDead { get; }
    bool IsVulnerable { get; }
    void Die();
}
}