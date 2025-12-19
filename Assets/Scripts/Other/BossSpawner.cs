using Enemies.Boss;
using Player;
using UnityEngine;

namespace Other
{
public class BossSpawner: MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject arenaEntrance; // Enable after entering arena to lock player in
    [SerializeField] private GameObject arenaExit; // Disable after boss is spawned to let player out
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float timeBeforeSpawn = 2f;

    private PlayerController _player;
    private bool _playerInArena;
    private float _bossSpawnTime;
    private bool _bossSpawned;

    private Boss _boss;

    private void Start() {
        _playerInArena = false;
        _bossSpawned = false;
    }

    private void FixedUpdate() {
        if (_bossSpawned || !_playerInArena)
            return;

        if (Time.time >= _bossSpawnTime) {
            SpawnBoss();
        }
    }

    private void SpawnBoss() {
        _boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Boss>();
        _boss.Target = _player;
        _boss.OnDeath += OnBossDeath;
        _bossSpawned = true;
        arenaEntrance.SetActive(true);
        arenaExit.SetActive(true);
    }

    private void OnBossDeath() {
        arenaEntrance.SetActive(false);
        arenaExit.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        bool hitPlayer = ((1 << other.gameObject.layer) & playerLayer) != 0;
        if (!hitPlayer)
            return;

        if (_player == null)
            _player = other.gameObject.GetComponent<PlayerController>();

        _playerInArena = true;
        _bossSpawnTime = Time.time + timeBeforeSpawn;
    }

    private void OnTriggerExit2D(Collider2D other) {
        bool hitPlayer = ((1 << other.gameObject.layer) & playerLayer) != 0;
        if (!hitPlayer)
            return;

        // easier operation than setting to null
        _playerInArena = false;
    }

}
}