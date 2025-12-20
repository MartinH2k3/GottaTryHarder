using Mechanics;
using Player;
using UnityEngine;
using Utils;

namespace Other
{
public class Car: MonoBehaviour, IPhysicsMovable
{
    [SerializeField] private Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int direction = 1; // 1 for right, -1 for left
    [SerializeField] private Animator animator;

    [SerializeField] private Vector2 playerKnockback = new(4, 4);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask terrainLayer;

    private Vector2 _velocity;

    // public event Action OnDespawn; // Maybe will be used later

    private void Awake() {
        if (animator != null) {
            int type = Random.Range(0, 3);
            animator.SetInteger("Type", type);
        }

        if (playerLayer == 0) {
            int playerLayerIndex = LayerMask.NameToLayer("Player");
            if (playerLayerIndex != -1) {
                playerLayer = 1 << playerLayerIndex;
            }
        }

        if (terrainLayer == 0) {
            int terrainLayerIndex = LayerMask.NameToLayer("Terrain");
            if (terrainLayerIndex != -1) {
                terrainLayer = 1 << terrainLayerIndex;
            }
        }

        playerKnockback.x *= direction;

        this.TurnAround(-direction); // sprites facing left by default
    }

    private void Start() {
        _velocity = new Vector2(direction * speed, 0);
    }

    private void FixedUpdate() {
        this.SetVelocity(_velocity);
    }

    private void OnCollisionEnter2D(Collision2D other) {

        if (!Helpers.LayerInLayerMask(other.gameObject.layer, playerLayer)) return;

        var player = other.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        player.AddForce(playerKnockback, ForceMode2D.Impulse);
        player.TakeDamage(1);

        // Ignore further collisions with the player
        var myColliders = GetComponentsInChildren<Collider2D>();
        var otherColliders = other.gameObject.GetComponentsInChildren<Collider2D>();

        foreach (var myCol in myColliders)
        {
            foreach (var otherCol in otherColliders)
            {
                if (myCol != null && otherCol != null)
                    Physics2D.IgnoreCollision(myCol, otherCol, true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (Helpers.LayerInLayerMask(other.gameObject.layer, terrainLayer)) {
            Destroy(gameObject);
        }
    }

}
}