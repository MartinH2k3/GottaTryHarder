using System;
using MyPhysics;
using Obstacles;
using Player.States;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using StateMachine = Infrastructure.StateMachine.StateMachine;

namespace Player
{

public class PlayerController: MonoBehaviour, IPhysicsMovable, IDamageable
{
    // controls
    public PlayerIntent Intent { get; } = new();
    private InputSystemActions _inputActions;
    private InputAction _move, _sprint, _jump, _dash, _interact;

    // states
    private StateMachine _stateMachine;
    private ControlState _controlState = ControlState.Normal;
    private VulnerabilityState _vulnerabilityState = VulnerabilityState.Vulnerable;
    private Idle _idle; private Walking _walking; private Airborne _airborne; private WallSliding _wallSliding; private Dashing _dashing;

    // state helpers
    public AirborneEntry LastAirborneEntry { get; private set; }

    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;
    private Collider2D _col;

    [Header("Health")]
    [SerializeField] private int maxHealthPoints = 100;
    private int _healthPoints;
    public bool IsDead { get; private set; }
    public bool IsVulnerable => _vulnerabilityState == VulnerabilityState.Vulnerable;
    public int HealthPoints { get; set; }

    [Header("Movement")]
    [SerializeField] private float moveEps = 0.1f; // deadzone for movement input
    public float walkSpeed = 3f;
    public float sprintMultiplier = 1.3f; // holding sprint button
    public float WalkSpeedMultiplier { get; set; } = 1f; // outside factors && effects
    public float walkAccel = 50f; // rates at which you reach walkSpeed
    public float walkDecel = 70f;

    [Header("Jump")]
    public float jumpStrength = 5f;
    [SerializeField] private float jumpTimeout = 0.3f;
    private float _nextJumpTime;

    public bool JumpOffCooldown => Time.time >= _nextJumpTime;
    public float LastJumpTime { get; set; } = float.NegativeInfinity;


    [Tooltip("Time AFTER leaving ground where jump is still allowed.")]
    [SerializeField] private float coyoteTimeWindow = 0.1f;
    private float _coyoteTimer;
    public bool HasCoyote => _coyoteTimer > 0f;

    [Tooltip("Time BEFORE getting on ground where jump is still allowed.")]
    [SerializeField] private float jumpBufferWindow = 0.1f;
    private float _jumpBufferTimer;
    public bool HasBufferedJump => _jumpBufferTimer > 0f;

    [Tooltip("Double/Triple jumps etc. Gets higher with upgrades, but serialized for testing.")]
    [SerializeField] private int extraAirJumps = 0; // 0 = no double jump
    private int _airJumpsLeft;
    public bool CanAirJump => _airJumpsLeft > 0;
    public void ConsumeAirJump() => _airJumpsLeft--;
    public void ResetAirJumps() => _airJumpsLeft = extraAirJumps;

    public bool CanSingleJump => IsGrounded || HasCoyote; // As in no need to spend double/triple/... jump
    public bool CanJump => JumpOffCooldown && (CanSingleJump || CanAirJump);
    public bool ShouldStartJump => HasBufferedJump && CanJump;
    public void ResetJumpCooldown() => _nextJumpTime = Time.time + jumpTimeout;
    public void ConsumeBufferedJump() => _jumpBufferTimer = 0f;
    public void ResetCoyote() => _coyoteTimer = coyoteTimeWindow;
    public void ConsumeCoyote() => _coyoteTimer = 0f;
    public void StartLandingMovementLock() => _horizontalControlUnlockTime = Time.time + landingMovementLockTime;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 1.5f; // max sliding speed
    public float wallSlideAccelX = 20f; // horizontal acceleration toward 0 while sliding
    public float wallStickTime = 0.25f; // time to stick to wall after stopping pushing into it before going airborne
    [SerializeField] private float upwardSpeedThreshold = 0.1f; // if going up faster than this, player wants to jump, so don't start sliding
    bool ShouldSlide => !IsGrounded && !WallRegrabLocked && TouchingWall && PressingIntoWall() && this.GetVelocity().y < upwardSpeedThreshold;


    [Header("Wall Jump")]
    public float wallJumpXStrength = 5f;
    public float wallJumpYStrength = 5f;
    public float wallRegrabLock = 0.5f; // time after jumping off wall before being able to regrab
    private float _wallRegrabUnlockTime = 0f;

    public void StartWallRegrabLock() => _wallRegrabUnlockTime = Time.time + wallRegrabLock;
    public bool WallRegrabLocked => Time.time < _wallRegrabUnlockTime;
    public void StartWallJumpControlLock(float duration = -1) => _horizontalControlUnlockTime = duration >= 0 ?
        Time.time + duration: Time.time + wallJumpControlLockTime;

    [Header("Ground contact")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    public bool IsGrounded =>Physics2D.OverlapCircle(
        new Vector2(transform.position.x, transform.position.y - PlayerHeight/2),
        groundCheckRadius, groundLayer);

    [Header("Wall contact")]
    [SerializeField] private LayerMask wallLayer;
    public bool TouchingWallLeft => Physics2D.OverlapArea(
        (Vector2)transform.position + new Vector2(-PlayerWidth/2, -(PlayerHeight - 0.05f)/2),
        (Vector2)transform.position + new Vector2(-PlayerWidth/2-0.01f, (PlayerHeight - 0.05f)/2),
        wallLayer);
    public bool TouchingWallRight => Physics2D.OverlapArea(
        (Vector2)transform.position + new Vector2(PlayerWidth/2, -(PlayerHeight - 0.05f)/2),
        (Vector2)transform.position + new Vector2(PlayerWidth/2+0.01f, (PlayerHeight - 0.05f)/2),
        wallLayer);
    public bool TouchingWall => TouchingWallLeft || TouchingWallRight;
    public int WallDir => TouchingWallLeft ? -1 : (TouchingWallRight ? 1 : 0);

    [Header("Air Control")]
    public float airSpeed = 3.0f; // horizontal
    public float airAccel = 35f;
    public float airDecel = 25f;
    public bool  allowSprintInAir = false;

    [Header("Horizontal lock")]
    [Tooltip("Time after wall jump before being able to control horizontal movement.")]
    [SerializeField] private float wallJumpControlLockTime = 0.05f;
    [Tooltip("Time after landing where horizontal movement is locked so it's easier to land on tight spaces.")]
    [SerializeField] private float landingMovementLockTime = 0.05f;
    private float _horizontalControlUnlockTime = 0f;
    public bool HorizontalControlLocked => Time.time <= _horizontalControlUnlockTime;

    [Header("Dashing")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private float _lastDashEndTime = float.NegativeInfinity;
    public void StartDashCooldown() => _lastDashEndTime = Time.time;

    [Header("Attack")]
    public float attackComboTime = 1f; // time window after attack to do another one
    private float _lastAttackTime = float.NegativeInfinity;
    public float jumpKickTime = 0.5f; // time window after jumping to do a jump kick
    public float jumpKickDamageMultiplier = 1.25f; // attack damage * this
    public float attackRange = 0.5f;
    public LayerMask attackableLayer;
    public float attackRate = 2f; // attacks per second
    private float _nextAttackTime = 0f;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("Misc")]
    public Animator animator;

    // Transform
    public int FacingDirection => transform.localScale.x >= 0 ? 1 : -1;
    public float PlayerHeight => _col.bounds.size.y;
    public float PlayerWidth  => _col.bounds.size.x;


    private void Awake() {
        _inputActions = new InputSystemActions();
        this.NeverSleep();
        _healthPoints = maxHealthPoints;
        _col = GetComponent<Collider2D>();
    }

    private void OnEnable() {
        var p = _inputActions.Player;
        _move = p.Move; _move.Enable();
        _sprint = p.Sprint; _sprint.Enable();
        _jump = p.Jump; _jump.Enable();
        _dash = p.Dash; _dash.Enable();
        _interact = p.Interact; _interact.Enable();

        _jump.performed += OnJumpPerformed;
        _dash.performed += OnDashPerformed;
        _interact.performed += context => Intent.InteractPressed = true;
    }

    private void OnDisable() {
        var p = _inputActions.Player;
        _move.Disable();
        _sprint.Disable();
        _jump.Disable();
        _dash.Disable();
        _interact.Disable();

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

        _stateMachine.Initialize(_idle);

        // Starting transitions
        _stateMachine.AddStartTransition(_idle, () => IsGrounded);
        _stateMachine.AddStartTransition(_wallSliding, () => ShouldSlide, priority: 2);
        _stateMachine.AddStartTransition(_airborne, () => !IsGrounded, priority: 1);

        // Walking <-> Idle
        _stateMachine.AddTransition(_idle, _walking,
            () => Intent.Move.sqrMagnitude > moveEps * moveEps);
        _stateMachine.AddTransition(_walking, _idle,
            () => Intent.Move.sqrMagnitude <= moveEps * moveEps);

        // -> Airborne: Jumping -- Higher priority than falling in case it happens on same frame
        _stateMachine.AddTransition(_idle, _airborne, () => ShouldStartJump, priority: 2);
        _stateMachine.AddTransition(_walking, _airborne, () => ShouldStartJump, priority: 2);

        // -> Airborne: Falling
        _stateMachine.AddTransition(_idle, _airborne, () => !IsGrounded, priority: 1);
        _stateMachine.AddTransition(_walking, _airborne, () => !IsGrounded, priority: 1);

        // Airborne -> WallSliding: Touching wall and pushing into it
        _stateMachine.AddTransition(_airborne, _wallSliding, () => ShouldSlide, 3);

        // Default Airborne exits
        _stateMachine.AddExitTransition(_airborne, () => IsGrounded);

        // WallSliding -> Airborne: Unsticking from wall when: not touching wall | jumping (to not cancel jump) |
        _stateMachine.AddTransition(_wallSliding, _airborne, () => !ShouldSlide && !_wallSliding.StickActive);
        // WallSliding -> Idle: Landing
        _stateMachine.AddExitTransition(_wallSliding, () => IsGrounded);

        _stateMachine.AddAnyTransition(_dashing, () => ShouldStartDash);
        _stateMachine.AddExitTransition(_dashing, () => ShouldStopDash);

        // Stuff to do on state changes
        _stateMachine.StateChanged += (prev, curr) =>
        {
            if (prev is Airborne && (curr == _idle || curr == _walking))
                StartLandingMovementLock();

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
        Intent.SprintHeld = _sprint.IsPressed();
        _stateMachine.Tick();
    }

    private void FixedUpdate() {
        _stateMachine.FixedTick();

        // Ground check
        if (IsGrounded) {
            _coyoteTimer = coyoteTimeWindow;
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
    }

    private void OnJumpPerformed(InputAction.CallbackContext context) {
        Intent.JumpPressed = true;
        Intent.LastJumpPressedTime = Time.time;
        _jumpBufferTimer = jumpBufferWindow;
    }

    private void OnDashPerformed(InputAction.CallbackContext context) {
        Intent.DashPressed = true;
        Intent.LastDashPressedTime = Time.time;
    }

    public bool PressingIntoWall() {
        float x = Intent.Move.x;
        return !Mathf.Approximately(x, 0) && ((TouchingWallLeft && x < 0) || (TouchingWallRight && x > 0));
    }

    private bool ShouldStartDash => Intent.DashPressed &&
                                    Time.time >= _lastDashEndTime + dashCooldown;
    private bool ShouldStopDash => Intent.LastDashPressedTime <= Time.time - dashDuration || // dash ran out;
                                   (FacingDirection > 0 ? TouchingWallRight : TouchingWallLeft);

    public void Die() {

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

    // Buffered press timestamps (for smoother experience)
    public float LastJumpPressedTime = float.NegativeInfinity;
    public float LastDashPressedTime  = float.NegativeInfinity;

    public void ClearFrameFlags()
    {
        JumpPressed = DashPressed = InteractPressed = false;
    }
}
}