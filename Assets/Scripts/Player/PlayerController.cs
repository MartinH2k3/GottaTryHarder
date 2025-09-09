using Infrastructure.StateMachine;
using Physics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine;

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

    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    [Header("Health")]
    [SerializeField] protected int maxHealthPoints = 100;
    protected int HealthPoints;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;

    private void Awake() {
        _inputActions = new InputSystemActions();
        this.NeverSleep();
        HealthPoints = maxHealthPoints;
    }

    public void TakeDamage(int damage) {
        HealthPoints -= damage;
    }

    public void Heal(int health) {
        HealthPoints += health;
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