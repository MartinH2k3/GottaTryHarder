using UnityEngine;

namespace Player.Stats
{
[System.Serializable]
public class JumpStats
{
    public float jumpStrength = 5f;
    public float jumpTimeout = 0.3f;
    [Tooltip("Time AFTER leaving ground where jump is still allowed.")]
    public float coyoteTimeWindow = 0.1f;
    [Tooltip("Time BEFORE getting on ground where jump is still allowed.")]
    public float jumpBufferWindow = 0.1f;
    [Tooltip("Double/Triple jumps etc. 0 = only single jump.")]
    public int extraAirJumps = 0; // 0 = no double jump

    [Header("Floating")]
    public float airSpeed = 3.0f; // horizontal
    public float airAccel = 35f;
    public float airDecel = 25f;
    [Tooltip("When rising below this speed, increase gravity (it's at the end of the jump when the jump feels floaty).")]
    public float gravityCutoffSpeed = 1f;
    public float downwardGravityMultiplier = 1.8f;
    public bool allowSprintInAir = false;

    [Header("Combat")]
    [Tooltip("Damage dealt when jumping from knee pain")]
    public int damageTakenOnJump = 0;
}
}