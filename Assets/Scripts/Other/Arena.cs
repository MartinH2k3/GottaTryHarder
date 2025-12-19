using System;
using Player;
using UnityEngine;

namespace Other
{
public abstract class Arena: MonoBehaviour
{
    [SerializeField] protected GameObject arenaEntrance; // Enable after entering arena to lock player in
    [SerializeField] protected GameObject arenaExit; // Disable after boss is spawned to let player out
    [SerializeField] protected float timeBeforeSpawn = 2f;
    [SerializeField] private LayerMask playerLayer;

    protected PlayerController Player;
    protected bool IsLocked;
    private bool _playerInArena;
    private float _spawnTime;
    private bool _spawned;

    protected virtual void Awake() {
        UnlockArena();
    }

    protected virtual void Start() {
        _playerInArena = false;
        _spawned = false;
    }

    protected virtual void FixedUpdate() {
        if (_spawned || !_playerInArena)
            return;

        if (Time.time >= _spawnTime) {
            Spawn();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        bool hitPlayer = ((1 << other.gameObject.layer) & playerLayer) != 0;
        if (!hitPlayer)
            return;

        if (Player == null)
            Player = other.gameObject.GetComponent<PlayerController>();

        _playerInArena = true;
        _spawnTime = Time.time + timeBeforeSpawn;
    }

    protected virtual void OnTriggerExit2D(Collider2D other) {
        bool hitPlayer = ((1 << other.gameObject.layer) & playerLayer) != 0;
        if (!hitPlayer)
            return;

        // easier operation than setting to null
        _playerInArena = false;
    }

    protected virtual void Spawn() {
        _spawned = true;
        LockArena();
    }

    protected virtual void LockArena() {
        arenaEntrance.SetActive(true);
        arenaExit.SetActive(true);
        IsLocked = true;
    }

    protected virtual void UnlockArena() {
        arenaEntrance.SetActive(false);
        arenaExit.SetActive(false);
        IsLocked = false;
    }
}
}