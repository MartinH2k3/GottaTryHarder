using Enemies.Boss.States;
using UnityEngine;

namespace Enemies.Boss
{
public class Boss: BaseEnemy
{
    private Idle _idle;
    private Dashing _dashing;
    private Bubbling _bubbling;

    private int _nextTransition; // 0: idle, 1: dash, 2: bubble

    private bool _shouldExitIdle;
    private bool _shouldExitDashing;
    private bool _shouldExitBubbling;

    public CircleCollider2D bubbleCollider;

    public System.Action OnDeath;

    protected override void Start() {
        base.Start();
        _idle = new Idle(this);
        _dashing = new Dashing(this);
        _bubbling = new Bubbling(this);
        StateMachine.Initialize(_idle);

        _idle.ShouldExit += () => _shouldExitIdle = true;
        _dashing.ShouldExit += () => _shouldExitDashing = true;
        _bubbling.ShouldExit += () => _shouldExitBubbling = true;

        StateMachine.AddStartTransition(_idle, ShouldStartIdle);
        StateMachine.AddStartTransition(_dashing, ShouldStartDashing);
        StateMachine.AddStartTransition(_bubbling, ShouldStartBubbling);

        StateMachine.AddExitTransition(_idle, ShouldExitState);
        StateMachine.AddExitTransition(_dashing, ShouldExitState);
        StateMachine.AddExitTransition(_bubbling, ShouldExitState);
    }

    public void EnableBubbleCollider() {
        bubbleCollider.enabled = true;
    }

    public void DisableBubbleCollider() {
        bubbleCollider.enabled = false;
    }

    public float GetBreakDuration() {
        return Random.Range(combatStats.actionCooldown - combatStats.actionCooldownRandomness,
            combatStats.actionCooldown + combatStats.actionCooldownRandomness);
    }

    public void PickNextTransition() {
        float dashOrBubble = Random.value;
        Debug.Log("Dash or Bubble roll: " + dashOrBubble);
        if (StateMachine.Current == _idle)
            _nextTransition = dashOrBubble < combatStats.dashProbability ? 1 : 2; // from idle, dash or bubble
        else if (StateMachine.Current == _dashing)
            _nextTransition = Random.value < combatStats.dashToBubbleProbability ? 2 : 0; // from dash, bubble or idle
        else if (StateMachine.Current == _bubbling)
            _nextTransition = 0; // after bubbling, always go to idle
    }

    private bool ShouldStartIdle() {
        return _nextTransition == 0;
    }

    private bool ShouldStartDashing() {
        return _nextTransition == 1;
    }

    private bool ShouldStartBubbling() {
        return _nextTransition == 2;
    }

    private bool ShouldExitState() {
        switch (StateMachine.Current) {
            case Idle _ when _shouldExitIdle:
                _shouldExitIdle = false;
                return true;
            case Dashing _ when _shouldExitDashing:
                _shouldExitDashing = false;
                return true;
            case Bubbling _ when _shouldExitBubbling:
                _shouldExitBubbling = false;
                return true;
            default:
                return false;
        }
    }

    public override void Die() {
        base.Die();
        OnDeath?.Invoke();
    }

    // Only applied for the bubbling state, as the main collider isn't a trigger
    private void OnTriggerEnter2D(Collider2D other) {
        if (StateMachine.Current == _bubbling) {
            _bubbling.HandleTriggerEnter(other);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other) {
        if (StateMachine.Current == _dashing) {
            _dashing.HandleCollisionEnter(other);
        } else if (StateMachine.Current == _idle) {
            _idle.HandleCollisionEnter(other);
        } else if (StateMachine.Current == _bubbling) {
            _bubbling.HandleCollisionEnter(other);
        }
    }
}
}