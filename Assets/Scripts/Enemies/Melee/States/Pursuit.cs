using Enemies.States;
using MyPhysics;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Pursuit: EnemyState
{
    public Pursuit(BaseEnemy enemy) : base(enemy) { }

    public override void FixedTick() {
        int direction = E.Target.position.x > E.Pos.x ? 1 : -1;

        if (E.CanWalkForward()) E.TurnAround();

        E.AccelerateHorizontally(direction * E.movementStats.chaseSpeed, E.movementStats.acceleration, direction);

    }
}
}