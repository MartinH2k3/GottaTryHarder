using System;
using Enemies.States;
using Enemies.Stats;
using Infrastructure.StateMachine;
using Managers;
using Mechanics;
using Player;
using UnityEngine;
using Utils;

namespace Enemies
{
public class BaseEnemy: MonoBehaviour, IDamageable, IPhysicsMovable
{
    // Stats. Would be too much effort with generic classes, so just put all stats here.
    [Header("Stats")]
    [Tooltip("Not all enemies will use all of the stats here.")]
    public MovementStats movementStats;
    [Tooltip("Not all enemies will use all of the stats here.")]
    public CombatStats combatStats;
    public int HealthPoints { get; set; }
    public bool IsDead { get; private set; } = false;

    /// <summary>To prevent lag, pathfinding happens in more scarce intervals, rather than every frame.</summary>
    [SerializeField] protected float detectionInterval = 0.2f;

    // States
    protected StateMachine StateMachine;
    protected Dying _deathState;

    // Targeting
    public PlayerController Target { get; set; }
    public Vector2 TargetPos => Target?.transform.position ?? Vector2.zero;
    protected float LastTargetCheckTime;
    protected bool LastTargetCheck;

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
        HealthPoints = combatStats.maxHealthPoints;
        Col = GetComponent<Collider2D>();
    }

    protected virtual void Start() {
        StateMachine = new StateMachine();
        _deathState = new Dying(this);
        StateMachine.AddAnyTransition(_deathState, () => IsDead);
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


    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        // Prevent enemy from pushing the player
        if (Helpers.LayerInLayerMask(collision.gameObject.layer, playerLayer)) {
            float stunDuration = Helpers.StunDurationEased(combatStats.knockbackStunDuration, combatStats.baseContactKnockbackStrength);
            Target.LockMovement(stunDuration);

            int knockbackDir = TargetPos.x - Pos.x > 0 ? 1 : -1;
            float knockback = combatStats.baseContactKnockbackStrength * combatStats.attackKnockback;
            Target.AddForce(knockbackDir*knockback, 3, ForceMode2D.Impulse);
        }
    }

    public virtual void TakeDamage(int damageAmount) {
        AudioManager.Instance.PlaySFX(sounds.hurt);
        HealthPoints -= damageAmount;
        if (HealthPoints <= 0) {
            Die();
        }
    }

    public virtual void Die() {
        IsDead = true;
    }

    public void DisableObject() {
        Col.enabled = false;
        rb.simulated = false;
    }

    public void DestroyGameObject () {
        Destroy(gameObject);
    }

    /// <summary>
    /// Default enemy behaviour is to check if the target is within range and within line of sight.
    /// Meant to be overriden by more complex enemies.
    /// </summary>
    protected virtual bool TargetInRange() {
            if (Time.time - LastTargetCheckTime < detectionInterval)
                return LastTargetCheck;

            // include player and terrain so obstacles will be detected before the player
            var mask = playerLayer | terrainLayer;
            var hit = Physics2D.Raycast(Pos,
                (TargetPos - Pos).normalized,
                combatStats.detectionRange,
                mask);

            LastTargetCheckTime = Time.time;
            LastTargetCheck = hit.collider is not null && (Vector2)hit.transform.position == TargetPos;

            return LastTargetCheck;
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