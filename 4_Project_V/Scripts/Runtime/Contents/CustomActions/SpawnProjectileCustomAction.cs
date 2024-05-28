using System;
using UnityEngine;

[Serializable]
public class SpawnProjectileCustomAction : CustomAction
{
    
    [SerializeField] private GameObject prefab;
    [SerializeField] private string spawnPointSocketName;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private GameObject _spawnObject;
    private Rigidbody _rb;
    private Vector3 _direction;

    public override void Start(object data)
    { 
        Spawn(data);
    }

    public override void Run(object data)
    {
        _rb.velocity = _direction.normalized * speed;
    }
    
    public override void Release(object data)
    {
        GameObject.Destroy(_spawnObject, lifeTime);
    }

    private void Spawn(object data)
    {
        if (data is Skill)
            Spawn(data as Skill);
        else
            Spawn(data as Effect);
    }
    
    private void Spawn(Skill data)
    {
        var socket = data.Owner.GetTransformSocket(spawnPointSocketName);
        _spawnObject = Managers.Resource.Instantiate(prefab, pooling: true);
        // _spawnObject = GameObject.Instantiate(prefab, socket.position, socket.rotation);
        _spawnObject.transform.position = socket.position;
        _spawnObject.transform.rotation = socket.rotation;
        _rb = _spawnObject.AddComponent<Rigidbody>();
        _direction = socket.forward;
    }

    private void Spawn(Effect data)
    {
        var socket = data.User.GetTransformSocket(spawnPointSocketName);
        // var projectile = GameObject.Instantiate(prefab, socket.position, socket.rotation);
        var projectile = Managers.Resource.Instantiate(prefab); 
        projectile.transform.position = socket.position;
        projectile.transform.rotation = socket.rotation;
    }
    
    public override object Clone()
    {
        return new SpawnProjectileCustomAction()
        {
            prefab = prefab,
            spawnPointSocketName = spawnPointSocketName,
            speed = speed,
            lifeTime = lifeTime
        };
    }
}