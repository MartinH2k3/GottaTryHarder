using System;
using UnityEngine;

namespace Other
{
public class CarSpawner: MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;

    private float _nextSpawnTime;

    private void Awake() {
        if (spawnPoint == null) {
            spawnPoint = transform;
        }
    }

    private void Start() {
        _nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update() {
        if (Time.time >= _nextSpawnTime) {
            SpawnCar();
            _nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnCar() {
        Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
}