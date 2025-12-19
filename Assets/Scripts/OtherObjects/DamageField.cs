using System;
using Mechanics;
using UnityEngine;
using Utils;

namespace OtherObjects
{
public class DamageField: MonoBehaviour
{
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private bool instantKill;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageInterval = 1f;
    private float _lastDamageTime;

    private void OnTriggerEnter2D(Collider2D other) {
        HandleCollider(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        HandleCollider(other);
    }

    private void HandleCollider(Collider2D other) {
        if (!Helpers.LayerInLayerMask(other.gameObject.layer, damageableLayers))
            return;

        Debug.Log(other.gameObject.name);
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
            return;

        if (instantKill) {
            damageable.Die();
            return;
        }

        if (Time.time - _lastDamageTime <= damageInterval)
            return;

        _lastDamageTime = Time.time;
        damageable.TakeDamage(damageAmount);
    }
}
}