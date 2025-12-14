using Mechanics;
using UnityEngine;
using Utils;

namespace Enemies.Golubok
{
public class GolubokProjectile: Poolable
{
    [SerializeField] private int damage = 5;
    [SerializeField] private float knockback = 1f;
    [SerializeField] private LayerMask playerLayer;

    private void OnCollisionEnter2D(Collision2D other) {
        if (((1 << other.gameObject.layer) & playerLayer) != 0) {
            var player = other.gameObject.GetComponent<Player.PlayerController>();
            if (player == null) return;

            Vector2 knockbackDir = (other.transform.position - transform.position).x > 0 ? Vector2.right : Vector2.left;
            player.AddForce(knockbackDir * knockback, ForceMode2D.Impulse);
            player.TakeDamage(damage);
        }

        Despawn();
    }
}
}