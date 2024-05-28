using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager
{
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    private void CreatePool(GameObject original, Transform parent = null)
    {
        Pool pool = new Pool(original) { Parent = parent };
        _pools.Add(original.name, pool);
    }

    public GameObject Pop(GameObject prefab, Transform parent = null)
    {
        if (!_pools.ContainsKey(prefab.name))
        {
            CreatePool(prefab, parent);
        }

        return _pools[prefab.name].Pop();
    }

    public bool Push(GameObject go)
    {
        if (!_pools.ContainsKey(go.name))
            return false;

        _pools[go.name].Push(go);

        return true;
    }

    public void Clear()
    {
        _pools.Clear();
    }
}

internal class Pool
{
    private GameObject _prefab;
    private IObjectPool<GameObject> _pool;
    private Transform _parent;

    public Transform Parent
    {
        get
        {
            if (_parent == null)
            {
                GameObject go = new GameObject($"{_prefab.name} Pool");
                _parent = go.transform;
            }

            return _parent;
        }
        set => _parent = value;
    }

    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public void Push(GameObject go)
    {
        if (go.activeSelf)
            _pool.Release(go);
    }

    public GameObject Pop()
    {
        return _pool.Get();
    }

    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);
        go.transform.SetParent(Parent);
        go.name = _prefab.name;

        return go;
    }

    private void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    private void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }

    private void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
}