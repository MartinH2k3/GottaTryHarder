using UnityEngine;

namespace Enemies.Stats
{
[System.Serializable]
public class MovementStats
{
    public float walkSpeed = 3f;
    public float chaseSpeed = 7f;
    public float acceleration = 30f;
    [Header("To avoid obstacles")]
    public float lookaheadDistance = 1f; // To avoid crashing or falling during a patrol
    public float groundDetectionDistance = 0.02f; // Offset from bottom of collider to detect ground
    [Header("Exclusive to golubok")]
    public float oscillationAmplitude = 0.5f;
    public float oscillationVelocity = 1f;
}
}