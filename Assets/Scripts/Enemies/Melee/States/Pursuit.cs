using Enemies.States;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Pursuit: EnemyState
{
    public Pursuit(BaseEnemy enemy) : base(enemy) { }

    public override void FixedTick() {
        base.FixedTick();
        Debug.Log("Pursuit: FixedTick");
    }
}
}