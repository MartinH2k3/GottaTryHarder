using System;
using Enemies.Boss.States;
using UnityEngine;
using Utils;

namespace Enemies.Boss
{
public class Boss: BaseEnemy
{
    private Idle _idle;
    private Dashing _dashing;
    private Bubbling _bubbling;

    public CircleCollider2D bubbleCollider;

    protected override void Start() {
        base.Start();
        _idle = new Idle(this);
        _dashing = new Dashing(this);
        _bubbling = new Bubbling(this);
        StateMachine.Initialize(_idle);
    }

    public void EnableBubbleCollider() {
        bubbleCollider.enabled = true;
    }

    public void DisableBubbleCollider() {
        bubbleCollider.enabled = false;
    }

    // Only applied for the bubbling state, as the main collider isn't a trigger
    private void OnTriggerEnter2D(Collider2D other) {
        if (StateMachine.Current == _bubbling) {
            _bubbling.HandleTriggerEnter(other);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
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