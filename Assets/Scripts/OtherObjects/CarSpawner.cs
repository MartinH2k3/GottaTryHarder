using UnityEngine;

namespace OtherObjects
{
public class CarSpawner: MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;
}
}