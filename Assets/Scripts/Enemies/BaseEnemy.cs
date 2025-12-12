using System;
using Enemies.Melee.Stats;
using Enemies.Stats;
using Infrastructure.StateMachine;
using Managers;
using Mechanics;
using Player;
using UnityEngine;

namespace Enemies
{
public class BaseEnemy: MonoBehaviour, IAttackable, IPhysicsMovable
{
    // Stats
    [Header("Stats")]
    [Tooltip("Not all enemies will use all of the stats here.")]
    public MovementStats movementStats;
    [Tooltip("Not all enemies will use all of the stats here.")]
    public CombatStats combatStats;
    [Tooltip("Not all enemies will use all of the stats here.")]
    public AnimationStats animationStats;
    private int _currentHealth;

    /// <summary>To prevent lag, pathfinding happens in more scarce intervals, rather than every frame.</summary>
    [SerializeField] private float detectionInterval = 0.2f;

    // States
    protected StateMachine StateMachine;

    // Targeting
    public PlayerController Target { get; set; }
    public Vector2 TargetPos => Target?.transform.position ?? Vector2.zero;
    private float _lastTargetCheckTime;
    private bool _lastTargetCheck;

    // Combat
    public float LastAttackTime { get; set; } = 0;

    // Physics
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;
    protected Collider2D Col;

    [Header("Layers")]
    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public LayerMask terrainLayer;

    // Helpers
    public int FacingDirection => transform.localScale.x >= 0 ? 1 : -1;
    public Vector2 Pos => transform.position;
    public float EnemyHeight => Col.bounds.size.y;
    public float EnemyWidth => Col.bounds.size.x;

    [Header("Audio")]
    public EnemySounds sounds;

    [Header("Misc")]
    public Animator animator;

    protected virtual void Awake() {
        _currentHealth = combatStats.maxHealthPoints;
        Col = GetComponent<Collider2D>();
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
        AudioManager.Instance.PlaySFX(sounds.hurt);
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0) {
            Die();
        }
    }

    public virtual void Die() {
        AudioManager.Instance.PlaySFX(sounds.death);
        Destroy(gameObject);
    }

    /// <summary>
    /// Checks if there is a ground to walk on ahead. Also checks for walls in the way.
    /// </summary>
    public bool CanWalkForward() {
        Vector2 positionAhead = Pos + FacingDirection * movementStats.lookaheadDistance * Vector2.right;

        RaycastHit2D groundHit = Physics2D.Raycast(positionAhead,
            Vector2.down,
            this.GetSizeY() / 2 + movementStats.groundDetectionDistance,
            terrainLayer);

        RaycastHit2D wallhit = Physics2D.Raycast(Pos,
            Vector2.right * FacingDirection,
            movementStats.lookaheadDistance,
            terrainLayer);

        // Visualize ground check ray (yellow)
        Debug.DrawRay(positionAhead,
            Vector2.down * (this.GetSizeY() / 2 + movementStats.groundDetectionDistance),
            Color.yellow);

        // Visualize wall check ray (red)
        Debug.DrawRay(Pos,
            Vector2.right * FacingDirection * movementStats.lookaheadDistance,
            Color.red);


        return groundHit.collider is not null && wallhit.collider is null;
    }

    /// <summary>
    /// Default enemy behaviour is to check if the target is within range and within line of sight.
    /// Meant to be overriden by more complex enemies.
    /// </summary>
    protected virtual bool TargetInRange() {
            if (Time.time - _lastTargetCheckTime < detectionInterval)
                return _lastTargetCheck;

            // include player and terrain so obstacles will be detected before the player
            var mask = playerLayer | terrainLayer;
            var hit = Physics2D.Raycast(Pos,
                (TargetPos - Pos).normalized,
                combatStats.detectionRange,
                mask);

            _lastTargetCheckTime = Time.time;
            _lastTargetCheck = hit.collider is not null && (Vector2)hit.transform.position == TargetPos;

            return _lastTargetCheck;
    }

    /// <summary>
    /// Checks if the target is within attack range. Meant to be overriden by more complex enemies.
    /// </summary>
    protected virtual bool TargetInAttackRange() {
        float distanceToTarget = Vector2.Distance(Pos, TargetPos);
        return distanceToTarget <= combatStats.attackRange;
    }
}


[Serializable]
public sealed class EnemySounds
{
    public AudioClip noticePlayer;
    public AudioClip attack;
    public AudioClip hurt;
    public AudioClip death;
}
}