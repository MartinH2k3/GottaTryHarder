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
    public bool  allowSprintInAir = false;
}
}