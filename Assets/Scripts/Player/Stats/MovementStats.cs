using UnityEngine;

namespace Player.Stats
{
[System.Serializable]
public class MovementStats
{
    public float moveEps = 0.1f; // deadzone for movement input
    public float walkSpeed = 4.5f;
    public float sprintMultiplier = 1.3f; // holding sprint button
    public float WalkSpeedMultiplier { get; set; } = 1f; // outside factors && effects
    public float walkAccel = 50f; // rates at which you reach walkSpeed
    public float walkDecel = 70f;
    [Tooltip("So the player doesn't slip accidentally when landing on tight platforms.")]
    public float onEdgeMovementLockTime = 0.05f;
    public float edgeRelockTime = 1f; // If you don't continue walking, the edge lock can retrigger again after this time
}
}