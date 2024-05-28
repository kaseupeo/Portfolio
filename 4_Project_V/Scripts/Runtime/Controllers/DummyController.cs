using System;
using UnityEngine.Events;

public class DummyController : BaseController
{
    public UnityEvent OnTakeDamage;

    private Creature _creature;

    private void Start()
    {
        _creature = GetComponent<Creature>();
        _creature.OnTakeDamage += TakeDamage;
    }

    private void TakeDamage(Creature creature, Creature instigator, object causer, float damage)
    {
        GetComponent<QuestReporter>().Report((int)damage);
    }
}