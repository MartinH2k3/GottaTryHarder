using Enemies.States;
using Mechanics;
using UnityEngine;

namespace Enemies.Melee.States
{
public class Attacking: EnemyState
{
    public Attacking(BaseEnemy enemy) : base(enemy) { }

    public bool IsAttackFinished => E.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    private float _attackStartTime;
    private bool _attackExecuted;

    public override void Enter() {
        base.Enter();



    }

    private void Attack() {
        E.animator.SetTrigger("Attack");
        var cornerA = (Vector2)E.transform.position +
                      new Vector2(0, -E.combatStats.attackHeight);
        var cornerB = (Vector2)E.transform.position +
                      new Vector2(E.combatStats.attackRange * E.FacingDirection,
                          E.combatStats.attackHeight);
        // Compute the axis-aligned rectangle from the two opposite corners and draw it for a short duration
        var min = new Vector2(Mathf.Min(cornerA.x, cornerB.x), Mathf.Min(cornerA.y, cornerB.y));
        var max = new Vector2(Mathf.Max(cornerA.x, cornerB.x), Mathf.Max(cornerA.y, cornerB.y));

        // if rectangle(min, max) overlaps with player collider (E.Target's collider), deal damage to player
        var player = E.Target;
        var playerCol = player.GetComponent<Collider2D>();
        bool playerWithinHitbox = playerCol.bounds.min.x <= max.x &&
                              playerCol.bounds.max.x >= min.x &&
                              playerCol.bounds.min.y <= max.y &&
                              playerCol.bounds.max.y >= min.y;
        if (playerWithinHitbox) {
            player.TakeDamage(E.combatStats.attackDamage);

        }
        DisplayAttackHitbox(min, max);
    }

    private void DisplayAttackHitbox(Vector2 min, Vector2 max) {
        Vector3 bl = new Vector3(min.x, min.y, 0f);
        Vector3 tl = new Vector3(min.x, max.y, 0f);
        Vector3 tr = new Vector3(max.x, max.y, 0f);
        Vector3 br = new Vector3(max.x, min.y, 0f);

        const float drawDuration = 1.15f; // seconds
        var color = Color.red;
        Debug.DrawLine(bl, tl, color, drawDuration);
        Debug.DrawLine(tl, tr, color, drawDuration);
        Debug.DrawLine(tr, br, color, drawDuration);
        Debug.DrawLine(br, bl, color, drawDuration);
    }
}
}


