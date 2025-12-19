using UnityEngine;

namespace Player.Stats
{
[System.Serializable]
public class WallSlideStats
{
    public float wallSlideSpeed = 1.5f; // max sliding speed
    public float wallSlideAccelX = 20f; // horizontal acceleration toward 0 while sliding
    public float wallStickTime = 0.25f; // time to stick to wall after stopping pushing into it before going airborne
    public float upwardSpeedThreshold = 0.1f;
    [Header("Wall Jump")]
    public float wallJumpXStrength = 5f;
    public float wallJumpYStrength = 6f;
    public float wallRegrabLock = 0.5f; // time after jumping off wall before being able to regrab
}
}