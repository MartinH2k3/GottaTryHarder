using Infrastructure.StateMachine;
using Physics;
using UnityEngine;

namespace Player.States
{
public class Idle: State
{
    private readonly PlayerController _player;

    public Idle(PlayerController player) => _player = player;

    public override void FixedTick() {
        base.FixedTick();
        var v = _player.GetVelocity();
        if (Mathf.Approximately(v.x, 0f)) {
            return;
        }
        float newX = Mathf.MoveTowards(v.x, 0f, _player.walkDecel * Time.fixedDeltaTime);
        _player.SetVelocity(newX, v.y);
    }
}
}