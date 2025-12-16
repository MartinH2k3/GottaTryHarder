using Managers;
using Mechanics;
using Player.Stats;
using UnityEngine;

namespace Player.States
{
public class Airborne: PlayerState
{
    private JumpStats _stats;

    private bool _hasIncreasedGravity;
    private float _ogGravityScale;

    public Airborne(PlayerController p) : base(p) {
        _stats = P.jumpStats;
    }

    public override void Enter() {
        base.Enter();
        _hasIncreasedGravity = false;
        if (P.Intent.LastJumpPressedTime <= Time.time - 0.3f)
            P.animator.SetTrigger("Fall");
    }

    public override void Exit() {
        base.Exit();
        AudioManager.Instance.PlaySFX(P.sounds.land);
        if (_hasIncreasedGravity)
            P.SetGravityScale(_ogGravityScale);
    }

    public override void FixedTick() {
        if (P.ShouldStartJump) Jump();

        var intent = P.Intent;
        if (Mathf.Approximately(intent.Move.x, 0)) return;

        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        var targetSpeed = horizontalInput *
                          _stats.airSpeed *
                          (intent.SprintHeld && _stats.allowSprintInAir ? P.movementStats.sprintMultiplier : 1f);
        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? _stats.airAccel : _stats.airDecel;

        P.AccelerateX(targetSpeed, acceleration);

        if (_hasIncreasedGravity || P.GetVelocity().y >= _stats.gravityCutoffSpeed)
            return;

        if (P.GetVelocity().y > 0f) // To get rid of that floaty jump at the top
            P.SetVelocityY(0f);

        _hasIncreasedGravity = true;
        _ogGravityScale = P.GetGravityScale();
        P.SetGravityScale(_ogGravityScale * _stats.downwardGravityMultiplier);

    }

    private void Jump() {
        AudioManager.Instance.PlaySFX(P.sounds.jump);
        if (!P.CanSingleJump && !P.CanAirJump)
            return;

        P.animator.SetTrigger("Jump");
        if (!P.CanSingleJump && P.CanAirJump)
            P.ConsumeAirJump();

        if (P.GetVelocity().y < 0f)
            P.SetVelocityY(0f); // Jumping stops falling

        P.AddForce(Vector2.up * _stats.jumpStrength, ForceMode2D.Impulse);
        P.ConsumeBufferedJump();
        P.ResetJumpCooldown();
        P.ConsumeCoyote();
        P.LastJumpTime = Time.time;
    }
}
}