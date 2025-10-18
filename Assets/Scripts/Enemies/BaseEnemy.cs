using MyPhysics;
using UnityEngine;

namespace Enemies
{
public class BaseEnemy: MonoBehaviour, IAttackable, IPhysicsMovable
{
    [SerializeField] private int maxHealth = 100;
    private int _currentHealth;

    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    protected virtual void Awake() {
        _currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damageAmount) {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0) {
            Die();
        }
    }

    public virtual void Die() {
        Destroy(gameObject);
    }
}
}