using Enemies.States;
using Managers;
using UnityEngine;

namespace Enemies.Golubok.States
{
public class Shooting: EnemyState<Golubok>
{
    public Shooting(Golubok enemy) : base(enemy) {}

    private bool ShootingAnimPlaying => E.animator.GetCurrentAnimatorStateInfo(0).IsName("Shooting");
    private bool AnimFinished => E.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    public bool AttackAnimFinished => AnimFinished && ShootingAnimPlaying;

    public override void Enter() {
        base.Enter();
        E.animator.Play("Charge Shoot");
    }

    public void Shoot() {
        E.animator.Play("Shooting");
        AudioManager.Instance.PlaySFX(E.sounds.attack);
        E.MissilePool.Spawn(E.Pos);
    }
}
}