using Enemies.States;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Attacking: EnemyState
{
    public Attacking(BaseEnemy enemy) : base(enemy) { }

    public bool IsAttackFinished => E.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;

    public override void Enter() {
        base.Enter();
        Debug.Log("Attack attack!");
    }
}
}