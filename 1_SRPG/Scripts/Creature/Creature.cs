using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// TODO
public abstract class Creature : MonoBehaviour
{
    [SerializeField] protected string name;
    [SerializeField] protected Sprite icon;
    protected int level;
    [SerializeField] protected List<Stat> stats;
    protected Stat currentStat;
    protected Define.State state;
    protected Tile currentTile;
    
    public string Name { get => name; set => name = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public int Level { get => level; set => level = value; }
    public List<Stat> Stats => stats;
    public Stat CurrentStat { get => currentStat; set => currentStat = value; }
    public Define.State State { get => state; set => state = value; }
    public Tile CurrentTile { get => currentTile; set => currentTile = value; }
    
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    
    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentStat = new Stat(stats[level]);
        state = Define.State.Idle;
        gameObject.name = name;
    }
    
    /*
     * 좌하단 방향 : -x, -y
     * 좌상단 방향 : -x, +y
     */
    public void Move(Tile targetTile)
    {
        float step = Managers.Game.GameSpeed * Time.deltaTime;
        float zIndex = targetTile.transform.position.z;
        currentTile.IsBlocked = false;
        
        transform.position = Vector2.MoveTowards(transform.position, targetTile.transform.position, step);
        transform.position = new Vector3(transform.position.x, transform.position.y, zIndex + 0.5f);

        Vector2 dir = targetTile.Grid2DLocation - currentTile.Grid2DLocation;
        if (dir.x != 0 || dir.y != 0)
        {
            animator.SetFloat("X", dir.x);
            animator.SetFloat("Y", dir.y);
            animator.SetBool("Walk", true);
        }
    }
    
    public virtual void CharacterPositionOnTile(Tile tile)
    {
        transform.position =
            new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z + 0.5f);
        spriteRenderer.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
        tile.IsBlocked = true;
        currentTile = tile;
    }
    
    public virtual void Attack(Creature target)
    {
        // TODO : 공격
        Vector2 dir = target.currentTile.Grid2DLocation - currentTile.Grid2DLocation;

        animator.SetFloat("X", dir.x);
        animator.SetFloat("Y", dir.y);
        int damage = 0;
        
        // 원거리
        if (dir.magnitude > 1)
        {
            damage = currentStat.LongRangeAttack - target.CurrentStat.Defence;
            animator.SetTrigger("Bow");
        }
        // 근접
        else
        {
            damage = currentStat.MeleeAttack - target.CurrentStat.Defence;
            animator.SetTrigger("Slash");
            Debug.Log($"{dir.magnitude}, target : {target.currentTile.Grid2DLocation}, player : {currentTile.Grid2DLocation}");
        }
        

        damage = damage > 0 ? damage : 0;

        target.currentStat.HealthPoint -= damage;
        Debug.Log($"{target.name} take damage : {damage}\nremaining hp : {target.currentStat.HealthPoint}");

        if (target.currentStat.HealthPoint <= 0)
        {
            target.currentStat.HealthPoint = 0;
            StartCoroutine(target.Dead());
        }
        else
        {
            target.animator.SetFloat("X", -dir.x);
            target.animator.SetFloat("Y", -dir.y);
            target.animator.SetTrigger("Damage");
        }
    }

    public void Healing(int point)
    {
        currentStat.HealthPoint += point;
        if (currentStat.HealthPoint > stats[level].HealthPoint)
        {
            currentStat.HealthPoint = stats[level].HealthPoint;
        }
    }

    public void AttackRankUp(float rate)
    {
        currentStat.MeleeAttack = (int)(currentStat.MeleeAttack * (rate / 100 + 1));
        currentStat.LongRangeAttack = (int)(currentStat.LongRangeAttack * (rate / 100 + 1));
    }

    protected virtual IEnumerator Dead()
    {
        state = Define.State.Dead;
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Dead");
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y -0.12f, currentPosition.z);
        
        for (var i = 0; i < 64; i++)
        {
            yield return null;
        }

        transform.position = currentPosition;
    }
}

[Serializable]
public class Stat
{
    [SerializeField] private int level;
    [SerializeField] private int healthPoint;
    [SerializeField] private int meleeAttack;
    [SerializeField] private int longRangeAttack;
    [SerializeField] private int defence;
    [SerializeField] private int turnCost;
    [SerializeField] private int attackCost;
    [SerializeField] private int minAttackRange;
    [SerializeField] private int maxAttackRange;

    public int Level { get => level; set => level = value; }
    public int HealthPoint { get => healthPoint; set => healthPoint = value; }
    public int MeleeAttack { get => meleeAttack; set => meleeAttack = value; }
    public int LongRangeAttack  { get => longRangeAttack; set => longRangeAttack = value; }
    public int Defence { get => defence; set => defence = value; }
    public int TurnCost { get => turnCost; set => turnCost = value; }
    public int AttackCost { get => attackCost; set => attackCost = value; }
    public int MinAttackRange { get => minAttackRange; set => minAttackRange = value; }
    public int MaxAttackRange { get => maxAttackRange; set => maxAttackRange = value; }

    public Stat(Stat stat)
    {
        level = stat.level;
        healthPoint = stat.healthPoint;
        meleeAttack = stat.meleeAttack;
        LongRangeAttack = stat.LongRangeAttack;
        defence = stat.defence;
        turnCost = stat.turnCost;
        attackCost = stat.attackCost;
        minAttackRange = stat.minAttackRange;
        maxAttackRange = stat.maxAttackRange;
    }
}