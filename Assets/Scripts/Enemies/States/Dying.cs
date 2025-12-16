using Managers;
using Mechanics;
using UnityEngine;

namespace Enemies.States
{
public class Dying: EnemyState {
    public Dying(BaseEnemy enemy) : base(enemy) { }

    private bool DeathAnimationFinished => E.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
                                           E.animator.GetCurrentAnimatorStateInfo(0).IsName("Death");

    public override void Enter() {
        base.Enter();
        E.SetVelocity(Vector2.zero);
        E.DisableObject();
        E.animator.Play("Death");
        AudioManager.Instance.PlaySFX(E.sounds.death);
    }

    public override void Tick() {
        base.Tick();
        if (DeathAnimationFinished) {
            E.DestroyGameObject();
        }
    }
    
}
}