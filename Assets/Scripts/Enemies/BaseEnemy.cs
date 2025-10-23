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

    /// <summary>To prevent lag, pathfinding happens in more scarce intervals, rather than every frame.</summary>
    [SerializeField] private float detectionInterval = 0.2f;

    // States
    protected StateMachine StateMachine;

    // Targeting
    public Transform Target { get; set; }
    private float _lastTargetCheckTime;
    private bool _lastTargetCheck;

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

    /// <summary>
    /// Checks if there is a ground to walk on ahead. Also checks for walls in the way.
    /// </summary>
    public bool CanWalkForward() {
        Vector2 positionAhead = Pos + FacingDirection * movementStats.lookaheadDistance * Vector2.right;

        RaycastHit2D groundHit = Physics2D.Raycast(positionAhead,
            Vector2.down,
            this.GetSizeX() / 2 + movementStats.groundDetectionDistance,
            terrainLayer);

        RaycastHit2D wallhit = Physics2D.Raycast(Pos,
            Vector2.right * FacingDirection,
            movementStats.lookaheadDistance,
            terrainLayer);

        return groundHit.collider is not null && wallhit.collider is null;
    }

    /// <summary>
    /// Default enemy behaviour is to check if the target is within range and within line of sight.
    /// Meant to be overriden by more complex enemies.
    /// </summary>
    protected virtual bool TargetInRange() {
            if (Target is null)
                return false;

            if (Time.time - _lastTargetCheckTime < detectionInterval)
                return _lastTargetCheck;

            // include player and terrain so obstacles will be detected before the player
            var mask = playerLayer | terrainLayer;
            var hit = Physics2D.Raycast(Pos,
                (Target.position - transform.position).normalized,
                movementStats.playerDetectionRange,
                mask);

            _lastTargetCheckTime = Time.time;
            _lastTargetCheck = hit.collider is not null && hit.collider.transform == Target;
            return _lastTargetCheck;

    }
}
}