using System;
using Enemies.States;
using UnityEngine;

namespace Enemies.Boss.States
{
public class BossState: EnemyState<Boss>
{
    protected BossState(Boss enemy) : base(enemy) {}

    public Action ShouldExit;

    public override void Enter() {
        base.Enter();
        E.PickNextTransition();
    }


}
}