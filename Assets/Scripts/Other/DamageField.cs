using System.Collections.Generic;
using Mechanics;
using UnityEngine;
using Utils;

namespace Other
{
public class DamageField : MonoBehaviour
{
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private bool instantKill;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageInterval = 1f;

    // Per-target cooldown
    private readonly Dictionary<Collider2D, float> _lastDamageTimeByTarget = new();

    private void OnDisable()
    {
        _lastDamageTimeByTarget.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other) => HandleCollider(other);
    private void OnTriggerStay2D(Collider2D other) => HandleCollider(other);

    private void OnTriggerExit2D(Collider2D other)
    {
        // Cleanup when target leaves
        _lastDamageTimeByTarget.Remove(other);
    }

    private void HandleCollider(Collider2D other)
    {
        if (!Helpers.LayerInLayerMask(other.gameObject.layer, damageableLayers))
            return;

        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
            return;

        if (instantKill) {
            damageable.Die();
            return;
        }

        _lastDamageTimeByTarget.TryGetValue(other, out var lastTime);

        if (Time.time - lastTime < damageInterval)
            return;

        _lastDamageTimeByTarget[other] = Time.time;
        damageable.TakeDamage(damageAmount);
    }
}
}