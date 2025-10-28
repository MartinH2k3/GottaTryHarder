using UnityEngine;

namespace Mechanics
{
public interface IDamageable
{
    int HealthPoints { get; set; }
    bool IsDead { get; }
    bool IsVulnerable { get; }
    void Die();
}
}