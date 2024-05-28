using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Threading;

public class ResourceManager
{
    // key : prefab path
    private Dictionary<string, Object> resources = new Dictionary<string, Object>();

    #region Resources.Load 방식

    public T Load<T>(string key) where T : Object
    {
        if (resources.TryGetValue(key, out Object resource))
            return resource as T;

        T t = Resources.Load<T>(key);
        resources.Add(key, t);
        
        return t;
    }

    public T[] LoadAll<T>(string key) where T : Object
    {
        T[] objects = Resources.LoadAll<T>(key);

        foreach (var obj in objects)
        {
            if (!resources.TryGetValue($"{key}/{obj.name}", out _))
            {
                resources.Add($"{key}/{obj.name}", obj);
            }
        }

        return objects;
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");

        if (prefab == null)
            return null;

        if (pooling)
            return Managers.Pool.Pop(prefab, parent);

        GameObject go = Object.Instantiate(prefab, parent);

        go.name = prefab.name;

        return go;
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null, bool pooling = false)
    {
        if (prefab == null)
            return null;
        
        if (pooling)
            return Managers.Pool.Pop(prefab, parent);

        GameObject go = Object.Instantiate(prefab, parent);

        go.name = prefab.name;

        return go;
    }
    
    public T Instantiate<T>(T original, Transform parent = null, bool pooling = false) where T : Object
    {
        if (original is GameObject gameObject)
        {
            if (pooling)
                return Managers.Pool.Pop(gameObject, parent) as T;
        }

        T t = Object.Instantiate(original, parent);

        t.name = original.name;

        return t;
    }
    
    public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent = null, bool pooling = false) where T : Object
    {
        if (original is GameObject gameObject)
        {
            if (pooling)
                return Managers.Pool.Pop(gameObject, parent) as T;
        }

        T t = Object.Instantiate(original, parent);

        t.name = original.name;

        return t;
    }

    public T Instantiate<T>(string path, Vector3 position, Quaternion rotation, Transform parent, bool pooling = false) where T : Object
    {
        T original = Load<T>(path);
        return Instantiate<T>(original, position, rotation, parent, pooling);
    }

    public T Instantiate<T>(string path, Vector3 position, Quaternion rotation, bool pooling = false) where T : Object
    {
        return Instantiate<T>(path, position, rotation, null, pooling);
    }

    public T Instantiate<T>(string path, Transform parent, bool pooling = false) where T : Object
    {
        return Instantiate<T>(path, Vector3.zero, Quaternion.identity, parent, pooling);
    }

    public T Instantiate<T>(string path, bool pooling = false) where T : Object
    {
        return Instantiate<T>(path, Vector3.zero, Quaternion.identity, null, pooling);
    }

    #endregion

    //public void Destroy(GameObject go)
    //{
    //    if (go == null)
    //        return;

    //    if (Managers.Pool.Push(go))
    //        return;

    //    Object.Destroy(go);
    //}
    public void Destroy(GameObject go)
    {
        if (Managers.Pool.IsContain(go))
            Managers.Pool.Release(go);
        else
            GameObject.Destroy(go);
    }

    public void Destroy(GameObject go, float delay)
    {
        if (Managers.Pool.IsContain(go))
            //TODO : 정상 작동이 안될 수 있음
            CoroutineHandler.Start_Coroutine(DelayReleaseRoutine(go, delay));
        else
            GameObject.Destroy(go, delay);
    }

    IEnumerator DelayReleaseRoutine(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.Pool.Release(go);
    }


    public void Clear()
    {
        resources.Clear();
    }

    
}