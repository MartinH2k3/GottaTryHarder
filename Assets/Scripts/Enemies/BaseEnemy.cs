using Enemies.Stats;
using Infrastructure.StateMachine;
using MyPhysics;
using UnityEngine;

namespace Enemies
{
public class BaseEnemy: MonoBehaviour, IAttackable, IPhysicsMovable
{
    // Stats
    [Tooltip("Not all enemies will use all of the stats here.")]
    [SerializeField] public MovementStats movementStats;
    [Tooltip("Not all enemies will use all of the stats here.")]
    [SerializeField] public CombatStats combatStats;
    private int _currentHealth;

    // States
    protected StateMachine StateMachine;

    // Physics
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    // Layers
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask terrainLayer;

    // Helpers
    public int FacingDirection => transform.localScale.x >= 0 ? 1 : -1;
    public Vector2 Pos => transform.position;

    protected virtual void Awake() {
        _currentHealth = combatStats.maxHealthPoints;
    }

    protected virtual void Start() {
        StateMachine = new StateMachine();
    }

    protected virtual void Update() {
        StateMachine.Tick();
    }

    protected virtual void FixedUpdate() {
        StateMachine.FixedTick();
    }

    protected void LateUpdate() {
        StateMachine.LateTick();
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