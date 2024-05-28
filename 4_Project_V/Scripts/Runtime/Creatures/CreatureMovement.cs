using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CreatureMovement : MonoBehaviour
{
    [SerializeField] private Stat moveSpeedStat;

    private NavMeshAgent _agent;
    private Transform _traceTarget;
    private Stat _creatureMoveSpeedStat;

    public Creature Owner { get; private set; }
    public NavMeshAgent Agent => _agent;
    public float MoveSpeed => _agent.speed;

    public Transform TraceTarget
    {
        get => _traceTarget;
        set
        {
            if (_traceTarget == value)
                return;

            Stop();
            _traceTarget = value;

            if (_traceTarget)
                StartCoroutine(CoTraceUpdate());
        }
    }

    //Controller에서 Destination 값을 직접 변경해줌
    public Vector3 Destination
    {
        get => _agent.destination;
        set
        {
            if (_agent.destination == value)
                return;

            SetDestination(value);
        }
    }

    public void Init(Creature owner)
    {
        Owner = owner;

        _agent = Owner.GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.autoBraking = false;

        _creatureMoveSpeedStat = moveSpeedStat ? Owner.Stats.GetStat(moveSpeedStat) : null;

        if (_creatureMoveSpeedStat)
        {
            _agent.speed = _creatureMoveSpeedStat.Value;
            _creatureMoveSpeedStat.OnValueChanged += ChangedMoveSpeed;
        }
    }

    private void OnEnable()
    {
       
    }
    private void OnDisable() => Stop();

    private void OnDestroy()
    {
        if (_creatureMoveSpeedStat)
            _creatureMoveSpeedStat.OnValueChanged -= ChangedMoveSpeed;
    }

    //agent에 Destination을 직접 전하는 함수. 
    private void SetDestination(Vector3 destination)
    {
        _agent.destination = destination;
        
        if (!Owner.IsPlayer || (Owner.IsPlayer && Managers.Input.RightMousePressAction.WasPressedThisFrame()))
        {
            
            LookAt(destination);
            return;
        }

        var lookDirection = (destination - transform.position).normalized;
        var rotation = lookDirection != Vector3.zero ? Quaternion.LookRotation(lookDirection) : transform.rotation;
        transform.rotation = rotation;
    }
    public void MonsterStop()
    {

        StopCoroutine(CoTraceUpdate());

        if (_agent.isOnNavMesh)
            _agent.ResetPath();

        _agent.velocity = Vector3.zero;
    }
    public void Stop()
    {
        _traceTarget = null;

        StopCoroutine(CoTraceUpdate());

        if (_agent.isOnNavMesh)
            _agent.ResetPath();

        _agent.velocity = Vector3.zero;
    }

    public void LookAt(Vector3 position)
    {
        StopCoroutine(nameof(CoLookAtUpdate));
        StartCoroutine(CoLookAtUpdate(position));
    }

    public void LookAtImmediate(Vector3 position)
    {
        position.y = transform.position.y;

        var lookDirection = (position - transform.position).normalized;
        var rotation = lookDirection != Vector3.zero ? Quaternion.LookRotation(lookDirection) : transform.rotation;

        transform.rotation = rotation;
    }

    private IEnumerator CoLookAtUpdate(Vector3 position)
    {
        position.y = transform.position.y;

        var lookDirection = (position - transform.position).normalized;
        var rotation = lookDirection != Vector3.zero ? Quaternion.LookRotation(lookDirection) : transform.rotation;
        // 회전하는 각도 / 걸리는 시간
        var speed = 180f / 0.15f;

        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, speed * Time.deltaTime);

            if (transform.rotation == rotation)
                break;

            yield return null;  
        }
    }

    private IEnumerator CoTraceUpdate()
    {
        while (true)
        {
            if (TraceTarget == null || !(Vector3.SqrMagnitude(TraceTarget.position - transform.position) > 1.0f))
                break;

            SetDestination(TraceTarget.position);

            yield return null;
        }
    }

    private void ChangedMoveSpeed(Stat stat, float currentValue, float prevValue)
        => _agent.speed = currentValue;


    #region AI Controller

    public bool InAttackRange { get; set; }
    public bool IsJumpping { get; set; }

    #endregion
}