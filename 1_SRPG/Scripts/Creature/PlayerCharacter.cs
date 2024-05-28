using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Creature
{
    private int _exp;
    private int _currentTurnCost;
    private int _currentAttackCost;
    private List<Item> _items;

    public int Exp { get => _exp; set => _exp = value; }
    public int CurrentTurnCost { get => _currentTurnCost; set => _currentTurnCost = value; }
    public int CurrentAttackCost { get => _currentAttackCost; set => _currentAttackCost = value; }
    public List<Item> Items { get => _items; set => _items = value; }
    
    public override void Init()
    { 
        base.Init();
        _items = new List<Item>();
        ResetTurnCost();
    }

    public void ResetTurnCost()
    {
        _currentTurnCost = currentStat.TurnCost;
        _currentAttackCost = currentStat.AttackCost;
    }
    
    public override void CharacterPositionOnTile(Tile tile)
    {
        base.CharacterPositionOnTile(tile);
        _currentTurnCost--;

        GainItem(tile);
    }

    public void LevelUpCheck()
    {
        int lv = _exp / 2;

        if (level == lv) 
            return;

        level++;
        if (stats.Count <= level)
        {
            level = stats.Count - 1;
            return;
        }
        
        currentStat = new Stat(stats[level]);
        Debug.Log($"{name} lv.{level} up");
    }

    public override void Attack(Creature target)
    {
        if (_currentAttackCost <= 0)
            return;
        
        base.Attack(target);
        _currentAttackCost--;
    }

    private void GainItem(Tile tile)
    {
        if (_items.Count > 10)
            return;
        
        foreach (var item in Managers.Game.FieldItems)
        {
            if (item.CurrentTile == tile)
            {
                _items.Add(item);
            }
        }

        foreach (Item item in _items)
        {
            Managers.Game.FieldItems.Remove(item);
            item.CurrentTile = null;
            item.gameObject.SetActive(false);
        }
    }

    protected override IEnumerator Dead()
    {
        yield return StartCoroutine(base.Dead());
        
        Managers.Game.PlayerCharacters.Remove(this);
    }

    private void OnDestroy()
    {
        currentTile.IsBlocked = false;
    }
}