using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager
{
    public Canvas canvasRoot;

    private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    private void CreatePool(GameObject original)
    {
        Pool pool = new Pool(original);
        pools.Add(original.name, pool);
    }

    public GameObject Pop(GameObject prefab, Transform parent = null)
    {
        if (!pools.ContainsKey(prefab.name))
            CreatePool(prefab);

        Pool pool = pools[prefab.name];
        pool.Parent = parent;

        return pool.Pop();
    }

    public bool Push(GameObject go)
    {
        if (!pools.ContainsKey(go.name))
            return false;

        pools[go.name].Push(go);

        return true;
    }

    public void Clear()
    {
        pools.Clear();
    }

    public bool Release<T>(T instance) where T : UnityEngine.Object
    {
        if (instance is GameObject)
        {
            GameObject go = instance as GameObject;
            string key = go.name;

            if (!pools.ContainsKey(key))
                return false;

            pools[key].Push(go);
            return true;
        }
        else if (instance is Component)
        {
            Component component = instance as Component;
            string key = component.gameObject.name;

            if (!pools.ContainsKey(key))
                return false;

            pools[key].Push(component.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsContain<T>(T original) where T : UnityEngine.Object
    {
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (pools.ContainsKey(key))
                return true;
            else
                return false;

        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (pools.ContainsKey(key))
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    public T GetUI<T>(T original, Vector3 position) where T : UnityEngine.Object
    {
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (!pools.ContainsKey(key))
                CreateUIPool(key, prefab);

            GameObject obj = pools[key].Pop();
            obj.transform.position = position;
            return obj as T;
        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (!pools.ContainsKey(key))
                CreateUIPool(key, component.gameObject);

            GameObject obj = pools[key].Pop();
            obj.transform.position = position;
            return obj.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    public T GetUI<T>(T original) where T : UnityEngine.Object
    {
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (!pools.ContainsKey(key))
                CreateUIPool(key, prefab);

            GameObject obj = pools[key].Pop();
            return obj as T;
        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (!pools.ContainsKey(key))
                CreateUIPool(key, component.gameObject);

            GameObject obj = pools[key].Pop();
            return obj.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    public bool ReleaseUI<T>(T instance) where T : UnityEngine.Object
    {
        if (instance is GameObject)
        {
            GameObject go = instance as GameObject;
            string key = go.name;

            if (!pools.ContainsKey(key))
                return false;

            pools[key].Push(go);
            return true;
        }
        else if (instance is Component)
        {
            Component component = instance as Component;
            string key = component.gameObject.name;

            if (!pools.ContainsKey(key))
                return false;

            pools[key].Push(component.gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateUIPool(string key, GameObject prefab)
    {
        Pool pool = new Pool(prefab);
           
        pools.Add(key, pool);
    }
}

internal class Pool
{
    private GameObject prefab;
    private IObjectPool<GameObject> pool;
    private Transform parent;

    public Transform Parent
    {
        get
        {
            if (parent == null)
            {
                GameObject go = new GameObject($"{prefab.name} Pool");
                parent = go.transform;
            }

            return parent;
        }
        set => parent = value;
    }

    public Pool(GameObject prefab)
    {
        this.prefab = prefab;
        pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public void Push(GameObject go)
    {
        if (go.activeSelf)
            pool.Release(go);
    }

    public GameObject Pop()
    {
        return pool.Get();
    }

    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(prefab);
        go.transform.SetParent(parent);
        go.name = prefab.name;

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
