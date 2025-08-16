using System.Collections.Generic;
using UnityEngine;
using Physics;

namespace Characters
{
public abstract class Character: MonoBehaviour, IPhysicsMovable {
    [SerializeField] protected int maxHealthPoints = 100;
    protected int healthPoints;
    [SerializeField] protected Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    // movement
    [SerializeField] protected float baseMovementSpeed = 3;
    protected float movementSpeed;

    private float _movementTimeout; // Unable to move after knockback or something alike
    private bool _isVulnerable = true; // Can take damage
    private bool _isSturdy = false; // Can't be moved by knockback, etc.

    protected virtual void Start() {
        healthPoints = maxHealthPoints;
        movementSpeed = baseMovementSpeed;
    }

    protected virtual void Update() {
        // cooldowns and timers handled here
    }

    protected void LateUpdate() {
        // crowd control will later be called here
    }

    // movement and velocity
    protected bool CanMove() {
        return _movementTimeout <= 0f;
    }

    public bool IsSturdy() {
        return _isSturdy;
    }

    public void SetSturdiness(bool canBeMoved) { //
        _isSturdy = canBeMoved;
    }

    public void SetMovementTimeout(float timeout) {
        _movementTimeout = timeout;
    }

    // Health

    private void SetVulnerability(bool isVulnerable) {
        _isVulnerable = isVulnerable;
    }

    public virtual void TakeDamage(int damage) {
        if (_isVulnerable) healthPoints -= damage;
    }

    public virtual void Heal(int health) {
        healthPoints += health;
    }

}
}