using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
public class GameObjectPool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Queue<GameObject> _available = new();

    private static readonly Dictionary<GameObject, GameObjectPool> _pools = new(); // Key: prefab, Value: pool, basically per prefab singleton

    private GameObjectPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public static GameObjectPool GetPool(GameObject prefab, Transform parent = null, int preWarm = 0)
    {
        if (!_pools.TryGetValue(prefab, out var pool))
        {
            pool = new GameObjectPool(prefab, parent);
            _pools[prefab] = pool;

            for (int i = 0; i < preWarm; i++)
            {
                var obj = pool.CreateNew();
                obj.SetActive(false);
                pool._available.Enqueue(obj);
            }
        }
        return pool;
    }

    public GameObject Spawn(Vector3 position)
    {
        var obj = _available.Count > 0 ? _available.Dequeue() : CreateNew();
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        _available.Enqueue(obj);
    }

    private GameObject CreateNew()
    {
        var obj = Object.Instantiate(_prefab, _parent);
        if (obj.TryGetComponent<Poolable>(out var poolable))
            poolable.SetPool(this);
        return obj;
    }

    public static void ClearAll() => _pools.Clear();
}

public abstract class Poolable : MonoBehaviour
{
    private GameObjectPool _pool;

    public void SetPool(GameObjectPool pool)
    {
        _pool = pool;
    }

    public void Despawn()
    {
        if (_pool != null)
            _pool.Despawn(gameObject);
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Called when spawned from pool. Override to reset state.
    /// </summary>
    public virtual void OnSpawn() { }

    /// <summary>
    /// Called when returned to pool. Override to clean up.
    /// </summary>
    public virtual void OnDespawn() { }
}
}