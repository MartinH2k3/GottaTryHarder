using Physics;
using Player.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using StateMachine = Infrastructure.StateMachine.StateMachine;

namespace Player
{

public class PlayerController: MonoBehaviour, IPhysicsMovable
{
    // controls
    public PlayerIntent Intent { get; } = new();
    private InputSystemActions _inputActions;
    private InputAction _move, _sprint, _jump, _dash, _interact;

    // states
    private StateMachine _stateMachine;
    private ControlState _controlState = ControlState.Normal;
    private VulnerabilityState _vulnerabilityState = VulnerabilityState.Vulnerable;
    private Idle _idle; private Walking _walking; private Airborne _airborne;

    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    [Header("Health")]
    [SerializeField] private int maxHealthPoints = 100;
    private int _healthPoints;

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

    [Tooltip("Time AFTER leaving ground where jump is still allowed.")]
    [SerializeField] private float coyoteTimeWindow = 0.1f;
    private float _coyoteTimer;

    [Tooltip("Time BEFORE getting on ground where jump is still allowed.")]
    [SerializeField] private float jumpBufferWindow = 0.1f;
    private float _jumpBufferTimer;

    [Tooltip("Double/Triple jumps etc. Gets higher with upgrades, but serialized for testing.")]
    [SerializeField] private int extraAirJumps = 0; // 0 = no double jump
    private int _airJumpsLeft;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 1.5f; // max sliding speed
    public float wallSlideAccelX = 20f; // horizontal acceleration toward 0 while sliding
    public float wallStickTime = 0.25f; // time to stick to wall after stopping pushing into it before going airborne

    [Header("Wall Jump")]
    public float wallJumpXStrength = 5f;
    public float wallJumpYStrength = 5f;
    public float wallRegrabLock = 0.2f; // time after jumping off wall before being able to regrab
    private float _wallRegrabUnlockTime = 0f;

    [Header("Ground contact")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;

    [Header("Wall contact")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    [SerializeField] private float wallCheckRadius = 0.1f;

    [Header("Air Control")]
    public float airSpeed = 3.0f; // horizontal
    public float airAccel = 35f;
    public float airDecel = 25f;
    public bool  allowSprintInAir = false;

    // Helpers for states
    // Ground check
    public bool IsGrounded =>Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    // Wall check
    public bool TouchingWallLeft => Physics2D.OverlapCircle(leftWallCheck.position, wallCheckRadius, wallLayer);
    public bool TouchingWallRight => Physics2D.OverlapCircle(rightWallCheck.position, wallCheckRadius, wallLayer);
    public bool TouchingWall => TouchingWallLeft || TouchingWallRight;
    public int WallDir => TouchingWallLeft ? -1 : (TouchingWallRight ? 1 : 0);
    // Multi jump
    public bool CanAirJump => _airJumpsLeft > 0;
    public void ConsumeAirJump() => _airJumpsLeft--;
    public void ResetAirJumps() => _airJumpsLeft = extraAirJumps;
    // Jump
    public bool HasBufferedJump => _jumpBufferTimer > 0f;
    public bool HasCoyote => _coyoteTimer > 0f;
    public bool JumpOffCooldown => Time.time >= _nextJumpTime;
    public bool CanSingleJump => IsGrounded || HasCoyote; // As in no need to spend double/triple/... jump
    public bool CanJump => JumpOffCooldown && (CanSingleJump || CanAirJump);
    public bool ShouldStartJump => HasBufferedJump && CanJump;
    public void ResetJumpCooldown() => _nextJumpTime = Time.time + jumpTimeout;
    public void ConsumeBufferedJump() => _jumpBufferTimer = 0f;
    public void ResetCoyote() => _coyoteTimer = coyoteTimeWindow;
    public void ConsumeCoyote() => _coyoteTimer = 0f;
    // Wall Jump
    public void StartWallRegrabLock() => _wallRegrabUnlockTime = Time.time + wallRegrabLock;
    public bool WallRegrabLocked => Time.time < _wallRegrabUnlockTime;


    private void Awake() {
        _inputActions = new InputSystemActions();
        this.NeverSleep();
        _healthPoints = maxHealthPoints;
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

        _stateMachine.Initialize(_idle);

        // Walking <-> Idle
        _stateMachine.AddTransition(_idle, _walking,
            () => Intent.Move.sqrMagnitude > moveEps * moveEps);
        _stateMachine.AddTransition(_walking, _idle,
            () => Intent.Move.sqrMagnitude <= moveEps * moveEps);

        // -> Airborne: Jumping
        _stateMachine.AddTransition(_idle, _airborne, () => ShouldStartJump, priority: 100);
        _stateMachine.AddTransition(_walking, _airborne, () => ShouldStartJump, priority: 100);

        // -> Airborne: Falling
        _stateMachine.AddTransition(_idle, _airborne, () => !IsGrounded, priority: 100);
        _stateMachine.AddTransition(_walking, _airborne, () => !IsGrounded, priority: 100);

        // Airborne -> Grounded: Landing
        _stateMachine.AddTransition(_airborne, _idle, () => IsGrounded);
    }

    private void Update() {
        Intent.Move = _move.ReadValue<Vector2>();
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

    public void TakeDamage(int damage) {
        _healthPoints -= damage;
    }

    public void Heal(int health) {
        _healthPoints += health;
    }
}

public enum ControlState { Normal, Stunned, Rooted, Sturdy}

public enum VulnerabilityState { Vulnerable, Invulnerable }


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