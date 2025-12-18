using System;
using Managers;
using Mechanics;
using Player.States;
using Player.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using StateMachine = Infrastructure.StateMachine.StateMachine;

namespace Player
{

public class PlayerController : MonoBehaviour, IPhysicsMovable, IDamageable
{
    // controls
    public PlayerIntent Intent { get; } = new();
    private InputSystemActions _inputActions;
    private InputAction _move, _sprint, _jump, _dash, _interact, _attack;

    public void DisableInput() => _inputActions.Disable();
    public void EnableInput() => _inputActions.Enable();


    // states
    private StateMachine _stateMachine;
    private ControlState _controlState = ControlState.Normal;
    private VulnerabilityState _vulnerabilityState = VulnerabilityState.Vulnerable;
    private Idle _idle;
    private Walking _walking;
    private Airborne _airborne;
    private WallSliding _wallSliding;
    private Dashing _dashing;
    private Attacking _attacking;

    [Header("References")] [SerializeField]
    protected Rigidbody2D rb;

    public Rigidbody2D Rigidbody => rb;
    private Collider2D _col;

    [Header("Movement")]
    public MovementStats movementStats;
    [ContextMenu("Reset Movement Stats")]
    public void ResetMovementStats() => movementStats = new MovementStats();

    [Header("Contact/Collision")]
    [SerializeField] private ContactStats contactStats;
    public LayerMask terrainLayer;

    public bool IsGrounded => Physics2D.OverlapCircle(
        new Vector2(transform.position.x, transform.position.y - PlayerHeight / 2),
        contactStats.groundCheckRadius, terrainLayer);

    private bool _touchingSide(int direction) {
        int touchingCount = 0;

        float x = direction * PlayerWidth / 2f;
        for (int i = 0; i < contactStats.contactPoints; i++) {
            if (Physics2D.OverlapCircle(
                    (Vector2)transform.position +
                    new Vector2(x,
                        -PlayerHeight / 2f + i / (Mathf.Max(contactStats.contactPoints - 1f, 1f)) * PlayerHeight),
                    contactStats.wallCheckRadius,
                    terrainLayer
                )
               ) touchingCount++;
        }
        return touchingCount >= contactStats.contactPoints * contactStats.contactRatio;
    }

    public bool TouchingWallLeft => _touchingSide(-1);
    public bool TouchingWallRight => _touchingSide(1);
    public bool TouchingWall => TouchingWallLeft || TouchingWallRight;
    public int WallDir => TouchingWallLeft ? -1 : (TouchingWallRight ? 1 : 0);

    [Header("Jump")]
    public JumpStats jumpStats;
    [ContextMenu("Reset Jump Stats")]
    public void ResetJumpStats() => jumpStats = new JumpStats();
    private float _nextJumpTime;
    public bool JumpOffCooldown => Time.time >= _nextJumpTime;
    public float LastJumpTime { get; set; } = float.NegativeInfinity;

    private float _coyoteTimer;
    public bool HasCoyote => _coyoteTimer > 0f;

    private float _jumpBufferTimer;
    public bool HasBufferedJump => _jumpBufferTimer > 0f;

    private int _airJumpsLeft;
    public bool CanAirJump => _airJumpsLeft > 0;
    public void ConsumeAirJump() => _airJumpsLeft--;
    public void ResetAirJumps() => _airJumpsLeft = jumpStats.extraAirJumps;

    public bool CanSingleJump => IsGrounded || HasCoyote; // As in no need to spend double/triple/... jump
    public bool CanJump => JumpOffCooldown && (CanSingleJump || CanAirJump);
    public bool ShouldStartJump => HasBufferedJump && CanJump;
    public bool ShouldStopJump => IsGrounded && Intent.LastJumpPressedTime < Time.time - 0.1f; // small delay to avoid cutting jump due to ground contact detection issues
    public void ResetJumpCooldown() => _nextJumpTime = Time.time + jumpStats.jumpTimeout;
    public void ConsumeBufferedJump() => _jumpBufferTimer = 0f;
    public void ResetCoyote() => _coyoteTimer = jumpStats.coyoteTimeWindow;
    public void ConsumeCoyote() => _coyoteTimer = 0f;

    [Header("Wall Slide")]
    public WallSlideStats wallSlideStats;
    [ContextMenu("Reset Wall Slide Stats")]
    public void ResetWallSlideStats() => wallSlideStats = new WallSlideStats();
    bool ShouldSlide => !IsGrounded && !WallRegrabLocked && TouchingWall && this.GetVelocity().y < wallSlideStats.upwardSpeedThreshold;

    // Wall Jump
    private float _wallRegrabUnlockTime = 0f;
    public void StartWallRegrabLock() => _wallRegrabUnlockTime = Time.time + wallSlideStats.wallRegrabLock;
    public bool WallRegrabLocked => Time.time < _wallRegrabUnlockTime;
    public void StartWallJumpControlLock(float duration = -1) => _horizontalControlUnlockTime = duration >= 0 ?
        Time.time + duration: Time.time + wallJumpControlLockTime;

    [Header("Dashing")]
    public DashStats dashStats;
    [ContextMenu("Reset Dash Stats")]
    public void ResetDashStats() => dashStats = new DashStats();

    private float _lastDashEndTime = float.NegativeInfinity;
    public void StartDashCooldown() => _lastDashEndTime = Time.time;

    private bool ShouldStartDash => Intent.DashPressed &&
                                    Time.time >= _lastDashEndTime + dashStats.dashCooldown;
    private bool ShouldStopDash => Intent.LastDashPressedTime <= Time.time - dashStats.dashDuration || // dash ran out;
                                   (FacingDirection > 0 ? TouchingWallRight : TouchingWallLeft);




    [Header("Combat")]
    public CombatStats combatStats;
    [ContextMenu("Reset Combat Stats")]
    public void ResetCombatStats() => combatStats = new CombatStats();
    public bool IsDead { get; private set; }
    public event Action OnDeath;
    public bool IsVulnerable => _vulnerabilityState == VulnerabilityState.Vulnerable;
    public int HealthPoints { get; set; }
    private float _invulnerabilityEndTime;

    private float _lastAttackTime = float.NegativeInfinity;
    public LayerMask attackableLayer;
    private float _nextAttackTime = 0f;

    public int ComboStep { get; private set; } = 0;
    public void ResetCombo() => ComboStep = 0;
    public void AdvanceCombo() => ComboStep = (ComboStep + 1) % 4;
    public bool ShouldAttack => Intent.AttackPressed && Time.time >= _nextAttackTime;


    [Header("Horizontal lock")]
    [Tooltip("Time after wall jump before being able to control horizontal movement.")]
    [SerializeField] private float wallJumpControlLockTime = 0.05f;
    [Tooltip("Time after landing where horizontal movement is locked so it's easier to land on tight spaces.")]
    private float _horizontalControlUnlockTime = 0f;
    public bool HorizontalControlLocked => Time.time <= _horizontalControlUnlockTime;

    [Header("Audio")]
    [SerializeField] public PlayerSounds sounds;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("Misc")]
    public Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [Tooltip("Flashing frequency when invulnerable.")]
    [SerializeField] private float invulnFlashHz = 12f;
    [Tooltip("Minimum opacity when invulnerable.")]
    [SerializeField] private float invulnMinAlpha = 0.25f;

    // Transform
    public int FacingDirection => transform.localScale.x >= 0 ? 1 : -1;
    public float PlayerHeight => _col.bounds.size.y;
    public float PlayerWidth  => _col.bounds.size.x;


    private void Awake() {
        _inputActions = new InputSystemActions();
        this.NeverSleep();
        HealthPoints = combatStats.maxHealthPoints;
        _col = GetComponent<Collider2D>();
    }

    private void OnEnable() {
        var p = _inputActions.Player;
        _move = p.Move; _move.Enable();
        _sprint = p.Sprint; _sprint.Enable();
        _jump = p.Jump; _jump.Enable();
        _dash = p.Dash; _dash.Enable();
        _interact = p.Interact; _interact.Enable();
        _attack = p.Attack; _attack.Enable();

        _jump.performed += OnJumpPerformed;
        _dash.performed += OnDashPerformed;
        _attack.performed += context => Intent.AttackPressed = true;
        _interact.performed += context => Intent.InteractPressed = true;
    }

    private void OnDisable() {
        var p = _inputActions.Player;
        _move.Disable();
        _sprint.Disable();
        _jump.Disable();
        _dash.Disable();
        _interact.Disable();
        _attack.Disable();

        _jump.performed -= OnJumpPerformed;
        _dash.performed -= OnDashPerformed;
    }

    private void Start() {
        _stateMachine = new StateMachine();
        _idle     = new Idle(this);
        _walking  = new Walking(this);
        _airborne = new Airborne(this);
        _wallSliding = new WallSliding(this);
        _dashing = new Dashing(this);
        _attacking = new Attacking(this);

        _stateMachine.Initialize(_idle);

        // Starting transitions
        _stateMachine.AddStartTransition(_idle, () => IsGrounded);
        _stateMachine.AddStartTransition(_wallSliding, () => ShouldSlide, priority: 2);
        _stateMachine.AddStartTransition(_airborne, () => !IsGrounded, priority: 1);

        // Walking <-> Idle
        _stateMachine.AddTransition(_idle, _walking,
            () => Intent.Move.sqrMagnitude > movementStats.moveEps * movementStats.moveEps);
        _stateMachine.AddTransition(_walking, _idle,
            () => Intent.Move.sqrMagnitude <= movementStats.moveEps * movementStats.moveEps);

        // -> Airborne: Jumping -- Higher priority than falling in case it happens on same frame
        _stateMachine.AddTransition(_idle, _airborne, () => ShouldStartJump, priority: 2);
        _stateMachine.AddTransition(_walking, _airborne, () => ShouldStartJump, priority: 2);

        // -> Airborne: Falling
        _stateMachine.AddTransition(_idle, _airborne, () => !IsGrounded, priority: 1);
        _stateMachine.AddTransition(_walking, _airborne, () => !IsGrounded, priority: 10);

        // Airborne -> WallSliding: Touching wall and pushing into it
        _stateMachine.AddTransition(_airborne, _wallSliding, () => ShouldSlide, 3);

        // Default Airborne exits
        _stateMachine.AddExitTransition(_airborne, () => ShouldStopJump);

        // Exiting WallSliding
        _stateMachine.AddExitTransition(_wallSliding, () => !ShouldSlide);

        // Attack
        _stateMachine.AddAnyTransition(_attacking, () => ShouldAttack);
        _stateMachine.AddExitTransition(_attacking, () => _attacking.IsAttackFinished);

        // Dashing
        _stateMachine.AddAnyTransition(_dashing, () => ShouldStartDash);
        _stateMachine.AddExitTransition(_dashing, () => ShouldStopDash);

        // Stuff to do on state changes
        _stateMachine.StateChanged += (prev, curr) =>
        {
            if (prev is Airborne && (curr == _idle || curr == _walking))
                LockMovement(movementStats.onEdgeMovementLockTime);

            if (prev is Walking && curr is Attacking)
                this.SetVelocityX(0);

            if (debugText)
                debugText.text = $"State: {_stateMachine.Current?.GetType().Name ?? "None"}\n" +
                                 $"Touching right wall: {TouchingWallRight}\n" +
                                 $"Touching left wall: {TouchingWallLeft}\n";
        };
    }

    private void Update() {
        Intent.Move = _move.ReadValue<Vector2>();
        if (HorizontalControlLocked)
            Intent.Move = new Vector2(0f, Intent.Move.y);

        if (_vulnerabilityState == VulnerabilityState.Invulnerable && Time.time >= _invulnerabilityEndTime)
            SetVulnerable();

        Intent.SprintHeld = _sprint.IsPressed();
        _stateMachine.Tick();
    }

    private void FixedUpdate() {
        _stateMachine.FixedTick();

        // Ground check
        if (IsGrounded) {
            _coyoteTimer = jumpStats.coyoteTimeWindow;
        } else {
            _coyoteTimer -= Time.fixedDeltaTime;
        }

        // Jump buffer timer
        if (_jumpBufferTimer > 0) {
            _jumpBufferTimer -= Time.fixedDeltaTime;
        }

        Intent.ClearFrameFlags();
    }

    private void LateUpdate() {
        _stateMachine.LateTick();

        if (!IsVulnerable)
            UpdateInvulnerabilityVisuals();
    }

    private void UpdateInvulnerabilityVisuals() {
        if (!sprite) return;

        // Ping-pong alpha between 1 and invulnMinAlpha
        float t = Mathf.PingPong(Time.time * invulnFlashHz, 1f);
        float a = Mathf.Lerp(invulnMinAlpha, 1f, t);
        var c = sprite.color;
        c.a = a;
        sprite.color = c;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context) {
        Intent.JumpPressed = true;
        Intent.LastJumpPressedTime = Time.time;
        _jumpBufferTimer = jumpStats.jumpBufferWindow;
    }

    private void OnDashPerformed(InputAction.CallbackContext context) {
        Intent.DashPressed = true;
        Intent.LastDashPressedTime = Time.time;
    }

    public void TakeDamage(int damage) {
        if (!IsVulnerable || IsDead) return;
        HealthPoints -= damage;
        SetInvulnerable();
        if (HealthPoints <= 0) {
            Die();
        }
    }

    public void Die() {
        if (IsDead) return;
        IsDead = true;
        AudioManager.Instance.PlaySFX(sounds.death);
        OnDeath?.Invoke();
    }

    public void LockMovement(float duration) {
        _horizontalControlUnlockTime = Time.time + duration;
    }

    private void SetInvulnerable() {
        _invulnerabilityEndTime = Time.time + combatStats.invulnerabilityDuration;
        _vulnerabilityState = VulnerabilityState.Invulnerable;
    }

    private void SetVulnerable() {
        _vulnerabilityState = VulnerabilityState.Vulnerable;
        var c = sprite.color;
        c.a = 1f;
        sprite.color = c;
    }

}

public enum ControlState { Normal, Stunned, Rooted, Sturdy}

public enum VulnerabilityState { Vulnerable, Invulnerable }

// Maybe will be used, but need to figure out a way to detect properly
public enum AirborneEntry { FromJump, FromFall, FromWallJump }

/// <summary> Abstracts handling player input into the intent of the input to be used by different states. </summary>
public sealed class PlayerIntent
{
    // Continuous input
    public Vector2 Move;
    public bool SprintHeld;

    // Triggered on frame
    public bool JumpPressed;
    public bool DashPressed;
    public bool InteractPressed;
    public bool AttackPressed;

    // Buffered press timestamps (for smoother experience)
    public float LastJumpPressedTime = float.NegativeInfinity;
    public float LastDashPressedTime  = float.NegativeInfinity;

    public void ClearFrameFlags()
    {
        JumpPressed = DashPressed = InteractPressed = AttackPressed = false;
    }
}

[Serializable]
public sealed class PlayerSounds
{
    public AudioClip jump;
    public AudioClip land;
    public AudioClip dash;
    public AudioClip attack;
    public AudioClip hurt;
    public AudioClip death;

    // Should disable constructor, as we never want to instantiate, but it gave me a warning and it annoyed me
    // private PlayerSounds() { }
}
}