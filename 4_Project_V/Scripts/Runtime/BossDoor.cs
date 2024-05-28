using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    Collider col;
    [SerializeField] LayerMask layer;
    private Creature boss;

    private void Start()
    {
        col = GetComponent<Collider>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(boss == null)
        {
           other.gameObject.TryGetComponent(out Creature boss);
            this.boss = boss;
            Debug.Log($"보스 네임 {this.boss.name}");
        }
        if(other.gameObject.layer == layer)
        {
            Managers.Battle.PlayerEnterBossRoom(boss);
        }
        
    }
}
