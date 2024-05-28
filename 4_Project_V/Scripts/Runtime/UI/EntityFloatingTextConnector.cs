using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFloatingTextConnector : MonoBehaviour
{
    [SerializeField]
    private Transform textSpawnPoint;

    private Creature _creature;    

    private void Start()
    {
        _creature = GetComponent<Creature>();
        _creature.OnTakeDamage += OnTakeDamage;
        _creature.StateMachine.OnStateChanged += OnStateChanged;
        _creature.Stats.HPStat.OnValueChanged += OnHPValueChanged;
    }

    private void OnTakeDamage(Creature creature, Creature instigator, object causer, float damage)
    {
        Managers.UI.FloatingTextView.Show(textSpawnPoint, $"-{Mathf.RoundToInt(damage)}", Color.red);
    }

    private void OnStateChanged(StateMachine<Creature> stateMachine, State<Creature> newState, State<Creature> prevState, int layer)
    {
        var ccState = newState as CreatureCCState;
        if (ccState == null)
            return;

        Managers.UI.FloatingTextView.Show(textSpawnPoint, ccState.Description, Color.magenta);
    }

    private void OnHPValueChanged(Stat stat, float currentValue, float prevValue)
    {
        var value = currentValue - prevValue;
        if (value > 0)
            Managers.UI.FloatingTextView.Show(textSpawnPoint, $"+{Mathf.RoundToInt(value)}", Color.green);
    }
}
