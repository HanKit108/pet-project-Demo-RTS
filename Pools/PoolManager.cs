using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PoolManager
{
    private const int INITIAL_COUNT = 10;

    private readonly Dictionary<int, Pool> _pools = new();
    [SerializeField]
    private List<Pool> _poolList = new();

    public Pool GetPool(Component prefab)
    {
        int id = prefab.GetInstanceID();
        return _pools.ContainsKey(id) ? _pools[id] : CreatePool(id, prefab);
    }

    private Pool CreatePool(int id, Component prefab)
    {
        var pool = new Pool(prefab, INITIAL_COUNT, prefab.name);
        _poolList.Add(pool);
        _pools[id] = pool;
        return pool;
    }
}
