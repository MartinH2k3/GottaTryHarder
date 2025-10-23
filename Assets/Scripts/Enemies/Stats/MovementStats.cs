using UnityEngine;

namespace Enemies.Stats
{
[System.Serializable]
public class MovementStats
{
    public float movementSpeed = 3f;
    [Header("To avoid obstacles")]
    public float lookaheadDistance = 1f; // To avoid crashing or falling during a patrol
    public float groundDetectionDistance = 0.02f; // Offset from bottom of collider to detect ground
}
}