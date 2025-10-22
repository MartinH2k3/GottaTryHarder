using Enemies.Stats;
using Infrastructure.StateMachine;
using MyPhysics;
using UnityEngine;

namespace Enemies
{
public class BaseEnemy: MonoBehaviour, IAttackable, IPhysicsMovable
{
    // Stats
    [SerializeField] public MovementStats movementStats;
    [SerializeField] public CombatStats combatStats;

    // States
    protected StateMachine StateMachine;

    // Health
    private int _currentHealth;

    // Physics
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    // Layers
    [SerializeField] public LayerMask playerLayer = LayerMask.GetMask("Player");
    [SerializeField] public LayerMask terrainLayer = LayerMask.GetMask("Terrain");

    // Helpers
    public int FacingDirection => transform.localScale.x >= 0 ? 1 : -1;
    public Vector2 Pos => transform.position;

    protected virtual void Awake() {
        _currentHealth = combatStats.maxHealthPoints;
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