using UnityEngine;

namespace Mechanics
{
public interface IDamageable
{
    int HealthPoints { get; set; }
    bool IsDead { get; }
    void TakeDamage(int damage);
    void Die();

}
}