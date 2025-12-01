using UnityEngine;

namespace Enemies.Melee.Stats
{
[System.Serializable]
public class AnimationStats
{
    [Tooltip("Normalized time in the attack animation when the damage is applied.")]
    public float damageApplyNormalizedTime = 0.8f;
}
}