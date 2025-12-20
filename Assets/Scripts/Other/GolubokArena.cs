using Enemies.Golubok;
using UnityEngine;

namespace Other
{
public class GolubokArena: Arena
{
    [SerializeField] private GameObject golubokPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject damageField;

    private int _remainingGoluboks;

    protected override void Awake() {
        base.Awake();
        damageField.SetActive(false);
    }

    protected override void Start() {
        base.Start();
        _remainingGoluboks = spawnPoints.Length;
    }

    private void Update() {
        if (IsLocked && _remainingGoluboks <= 0) {
            UnlockArena();
        }
    }

    protected override void Spawn() {
        base.Spawn();
        foreach (var point in spawnPoints) {
            var golubok = Instantiate(golubokPrefab, point.position, Quaternion.identity).GetComponent<Golubok>();
            golubok.Target = Player;
            golubok.OnDeath += () => _remainingGoluboks--;

        }
    }

    protected override void LockArena() {
        base.LockArena();
        damageField.SetActive(true);
    }

    protected override void UnlockArena() {
        base.UnlockArena();
        damageField.SetActive(false);
    }
}
}