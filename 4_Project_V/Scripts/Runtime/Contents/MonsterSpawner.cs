using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] monsterPrefabs;
    [SerializeField] private int count;
    [SerializeField] private float range;
    [SerializeField, Range(1, 100)] private float time;

    private int _count;
    
    private void Start()
    {
        Spawns(count);
        _count = count;
    }
    
    private void Spawns(int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Spawn(monsterPrefabs[Random.Range(0, monsterPrefabs.Length)]);
        }
    }
    
    private GameObject Spawn(GameObject prefab)
    {
        _count += 1;
        var spawnPosition = Random.insideUnitCircle * range;
        GameObject go = Managers.Resource.Instantiate(prefab, pooling: true);
        go.GetComponent<NavMeshAgent>().enabled = false;
        var position = transform.position;
        go.transform.position = new Vector3(position.x + spawnPosition.x, position.y, position.z + spawnPosition.y);
        go.GetComponent<NavMeshAgent>().enabled = true;
        go.GetComponent<Creature>().Init();
        go.GetComponent<Creature>().OnDead += _ =>
        {
            _count -= 1;
            StartCoroutine(Respawn());
        };

        return go;
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(time);

        if (_count >= count)
            yield break;
        
        GameObject go = Spawn(monsterPrefabs[Random.Range(0, monsterPrefabs.Length)]);
    }
}