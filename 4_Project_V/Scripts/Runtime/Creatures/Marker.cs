using System;
using UnityEngine;
using UnityEngine.AI;

public class Marker : MonoBehaviour
{
    [SerializeField] private GameObject marker;

    private Creature _player;
    
    private void Start()
    {
        _player = FindObjectOfType<Creature>();
    }

    private void Update()
    {
        if (Vector3.Distance(_player.Movement.Destination, transform.position) < 0.1f)
        {
            gameObject.SetActive(false);
        }
    }
}