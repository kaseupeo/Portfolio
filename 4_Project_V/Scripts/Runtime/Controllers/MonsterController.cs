using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.AI;
using System.Collections;
using System.Linq;


public enum CommandState
{
    BasicSkill,
    Skill2,
    Skill3,
    Skill4,
}

[RequireComponent(typeof(AISensor))]
public class MonsterController : CreatureController
{

    public CommandState commandState;



    [SerializeField] public Stat attackRange;

    private Creature creature;
    private Transform traceTarget;
    private Transform target => creature.Movement.TraceTarget;
    public AISensor sensor;
    public NavMeshAgent Agent => creature.Movement.Agent;
    public Rigidbody rb;
    public Collider col;

    #region Wander에 필요한 변수들
    //생성되는 시점에 최대 최소 움직일 수 있는 거리를 알고 있도록 하는 변수들
    public Vector3 defaultAvailabilityToWander = new Vector3(20f, 0, 20f);
    private Vector3 minBound;
    private Vector3 maxBound;
    #endregion

    public float moveSpeed => creature.Movement.MoveSpeed;

    public bool HasTarget => sensor.hasTarget;

    [SerializeField] private bool inAttackRange;
    public bool InAttackRange
    {
        get => inAttackRange;
        set
        {
            if (inAttackRange == value) return;

            inAttackRange = value;
            creature.Movement.InAttackRange = inAttackRange;
        }
    }

    private bool isJump;

    private bool jumpAction;
    public bool IsJumpDashAction
    {
        get => jumpAction;
        private set
        {
            if (jumpAction == value) return;
            jumpAction = value;
            creature.Movement.IsJumpping = jumpAction;
        }
    }

    private int basicAttackCount;
    private int skill2Count;
    private int skill3Count;

    private void Awake()
    {
        sensor = GetComponent<AISensor>();
        creature = GetComponent<Creature>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        col = GetComponent<Collider>();
    }
    private void Start()
    {
      
    }

    private void Update()
    {
        if (creature.IsDead)
            return;     
        //CheckDot(Agent.destination);

        if (HasTarget)
        {
            creature.Target = sensor.TargetPlayer.GetComponent<Creature>();
        }
    }


    private void OnEnable()
    {
        minBound = transform.position - defaultAvailabilityToWander;
        maxBound = transform.position + defaultAvailabilityToWander;
        //creature.Target = creature;
        creature.OnDead += Dead;
    }

    private void OnDisable()
    {
        creature.OnDead -= Dead;
    }

    private Skill SelectSkill(Skill skill = null)
    {
        if (skill == null)
        {
            foreach (Skill s in creature.SkillSystem.ActiveSkillList)
            {
                if (s.IsUseable)
                    return s;
            }
        }
        else if (skill.IsUseable) return skill;
        return null;
    }
    public void Attack()
    {
        var selectSkill = SelectSkill();
        selectSkill.Use();
    }
    public void Attack(CommandState state)
    {
        var selectSkill = SelectEnumSkill((int)state);

        if (selectSkill == null)
        {
            selectSkill = SelectSkill();
        }
        if (!selectSkill.IsUseable) return;

        switch (state)
        {
            case CommandState.BasicSkill:
                basicAttackCount++;
                Debug.Log($"basicAttackCount : {basicAttackCount}");
                break;
            case CommandState.Skill2:
                skill3Count = 0;
                skill2Count++;
                Debug.Log($"skill2Count : {skill2Count}");
                break;
            case CommandState.Skill3:
                skill2Count = 0;
                skill3Count++;
                Debug.Log($"skill3Count : {skill3Count}");
                break;

        }
        selectSkill.Use();

    }
    public void AttackState()
    {
        if (basicAttackCount <= 3 && skill2Count < 1)
        {
            Attack1();
        }
        else if (basicAttackCount > 3 && skill2Count == 0)
        {
            Attack2();
        }
        else if (basicAttackCount > 3 && skill3Count == 0)
        {
            Attack3();
        }
       
    }
    public void Attack1()
    {
        Attack(CommandState.BasicSkill);
      
    }
    public void Attack2()
    {
        Attack(CommandState.Skill2);
    }

    public void Attack3()
    {
        Attack(CommandState.Skill3);
    }

    private Skill SelectEnumSkill(int num) => creature.SkillSystem.ActiveSkillList[num];

    public void GenWandersPosition()
    {
        float randPosX = Random.Range(minBound.x, maxBound.x);
        float randPosZ = Random.Range(minBound.z, maxBound.z);
        Vector3 dest = new Vector3(randPosX, transform.position.y, randPosZ);
        MoveToDestination(dest);
    }
    public void MoveToDestination(Vector3 dest)
    {
        if (IsAlmostReached())
        {
            creature.Movement.Stop();
        }
        // CheckDot(dest);
        //creature.Movement.LookAt(dest);

        //Movement사용
        // CheckDot(dest);
        creature.Movement.Destination = dest;

    }

    //Movement를 통해 움직이게될 경우 사용 안할 예정
    public bool CheckDot(Vector3 position)
    {
        position.y = transform.position.y;
        Vector3 lookDirection = (position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, lookDirection);
        if (dot < 0.99f)
        {
            return true;
        }
        return false;
    }
    public bool IsAlmostReached()
    {
        bool isAlmostReached = true;
        if (Agent.hasPath)
        {
            //TODO: 나중에 공격 사거리가 있다면 수정할 것
            isAlmostReached = Agent.remainingDistance > 0.2f ? true : false;
        }
        return isAlmostReached;
    }

    public void StartJump()
    {
        if (HasTarget && !isJump)
        {
            if (target == null) creature.Movement.TraceTarget = sensor.TargetPlayer.transform;
            StopCoroutine(nameof(JumpTowardsToTarget));
            IsJumpDashAction = true;
            StartCoroutine(JumpTowardsToTarget());
        }
    }

    private IEnumerator JumpTowardsToTarget()
    {

        bool done = false;
        float prevPosY = transform.position.y;
        while (!done)
        {
            if (Vector3.Distance(transform.position, target.position) < 10f && !isJump)
            {
                float fixedYPos = transform.position.y;
                Vector3 targetPos = new Vector3(target.position.x, fixedYPos, target.position.z);
                Vector3 lookDirection = (targetPos - transform.position).normalized;
                var rotation = lookDirection != Vector3.zero ? Quaternion.LookRotation(lookDirection) : transform.rotation;
                transform.rotation = rotation;
                if (transform.rotation != rotation) yield return null;
                Vector3 jumpVector = (target.position - transform.position).normalized;
                Vector3 testVector = jumpVector + Vector3.up;
                isJump = true;
                kinematicSwitch(false);

                rb.AddForce(testVector * 10, ForceMode.Impulse);
                yield return new WaitForSeconds(1.8f);

            }
            #region 이부분 레이케스트 체크 안됨
            if (isJump)
            {
                Debug.Log("isJump");
                Debug.DrawRay(transform.position, Vector3.down, Color.red, prevPosY + 2f);
                var raycast = Physics.RaycastAll(transform.position, Vector3.down, prevPosY + 1.9f, LayerMask.GetMask("Ground"));
                //print($"땅바닥 감지{rayHit.collider.name}");
                if (raycast.Count() > 0)
                {
                    print($"{raycast[0].collider.name}");

                    isJump = false;
                    done = true;
                }
            }
            #endregion
            yield return null;
        }
        isJump = false;
        kinematicSwitch(true);
        done = false;
        IsJumpDashAction = false;
    }
    private bool kinematicSwitch(bool value)
    {
        if (!value)
        {
            Agent.ResetPath();
        }
        Agent.enabled = value;
        creature.Movement.enabled = value;
        col.isTrigger = value;
        rb.isKinematic = value;
        return value;
    }
    public bool KinematickSwitch(bool value)
    {
        return kinematicSwitch(value);
    }

    private void Dead(Creature creature)
    {
        if (!Managers.Pool.Push(gameObject))
        {
            Debug.Log("몬스터 디스폰 에러");
            gameObject.SetActive(false);
        }


        OnDead.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, 4);
    }

}