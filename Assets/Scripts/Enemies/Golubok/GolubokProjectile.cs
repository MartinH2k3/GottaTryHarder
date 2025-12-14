using Mechanics;
using UnityEngine;
using Utils;

namespace Enemies.Golubok
{
public class GolubokProjectile: Poolable
{
    [SerializeField] private int damage = 5;
    [SerializeField] private LayerMask playerLayer;

    private void OnCollisionEnter2D(Collision2D other) {
        if (((1 << other.gameObject.layer) & playerLayer) != 0) {
            var player = other.gameObject.GetComponent<Player.PlayerController>();

            if (player != null)
                player.TakeDamage(damage);
        }
        Despawn();
    }
}
}