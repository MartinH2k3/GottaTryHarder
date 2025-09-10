using Infrastructure.StateMachine;
using Physics;
using UnityEngine;

namespace Player.States
{
public class Walking: State
{
    private readonly PlayerController _player;

    public Walking(PlayerController player) {
        _player = player;
    }

    public override void FixedTick() {
        var intent = _player.Intent;
        var horizontalInput = Mathf.Sign(intent.Move.x) * intent.Move.magnitude;

        var targetSpeed = horizontalInput *
                          _player.walkSpeed *
                          (intent.SprintHeld ? _player.sprintMultiplier : 1f) *
                          _player.WalkSpeedMultiplier;

        var acceleration = Mathf.Abs(targetSpeed) > 0.01f ? _player.walkAccel : _player.walkDecel;

        _player.AccelerateHorizontally(targetSpeed, acceleration, horizontalInput);
    }

}
}