using Enemies.Boss;
using Player;
using UnityEngine;

namespace Other
{
public class BossArena: Arena
{
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform spawnPoint;

    private Boss _boss;

    protected override void Spawn() {
        base.Spawn();
        _boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity).GetComponent<Boss>();
        _boss.Target = Player;
        _boss.OnDeath += OnBossDeath;
    }

    private void OnBossDeath() {
        UnlockArena();
    }

}
}