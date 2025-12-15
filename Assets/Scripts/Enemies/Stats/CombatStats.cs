using UnityEngine;

namespace Enemies.Stats
{
[System.Serializable]
public class CombatStats
{
    public int maxHealthPoints = 30;
    public int attackDamage = 10;
    public float attackKnockback = 2f;
    public float knockbackStunDuration = 0.5f;
    [Tooltip("Normalized time in the attack animation when the damage is applied.")]
    public float damageApplyNormalizedTime = 0.8f;

    [Header("Targeting")]
    public float noticeTime = 0.5f; // Time for the enemy to process in their head "Oh wait, there is a guy, I'm going after him"
    public float detectionRange = 10f;
    [Tooltip("How often to run the algorithm.")]
    public float detectionPeriod = 0.2f;

    [Header("Melee specific")]
    public float attackWidth = 0.2f;
    public float verticalKnockback = 1f;
    [Tooltip("Base attack speed is the length of the attack animation.")]
    public float attackSpeedMult = 1f;
    public float attackRange = 1f;

    [Header("Golubok specific")]
    public float launchChargeTime = 1f;
    public float launchForce = 10f;
    public float bounceFactor = 0.8f;
    [Tooltip("Angle which the Golubok will approach when bouncing off the player. Off surfaces, he bounces off normally.")]
    public float bounceOffAngle = 70f;
    [Tooltip("Duration of the launch state after launching.")]
    public float launchDuration = 1.8f;

    [Header("Boss specific")]
    public float maxBubbleSize = 1.5f;
    public float idleKnockbackStrength = 1.5f;
    public float bubbleKnockbackStrength = 2.6f;
    public float dashKnockbackStrength = 4.5f;
    [Tooltip("How many seconds between boss actions (dashes, bubbles, etc.)")]
    public float actionCooldown = 5f;
    [Tooltip("Randomness to action cooldown to avoid predictability.")]
    public float actionCooldownRandomness = 2f;
    [Tooltip("Probability of dashing vs bubbling.")]
    public float dashProbability = 0.6f;
    [Tooltip("Probability of going immediately from dash to bubble without idle in between.")]
    public float dashToBubbleProbability = 0.3f;
    public float dashDuration = 1f;
    public float dashSpeed = 8f;

}
}