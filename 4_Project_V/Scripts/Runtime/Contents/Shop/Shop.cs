using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private SO_Item[] items;
    [SerializeField] private Stat goldStat;
    [SerializeField] private float gold;

    public IReadOnlyList<SO_Item> Items => items;

    public float Gold { get => gold; set => gold = value; }
    
    private void Awake()
    {
        Managers.Game.Shop = this;
    }

    private void Init()
    {
        // TODO : 데이터 입력
        
    }

    // 플레이어가 상점 아이템 구매
    public bool Buy(SO_Item item)
    {
        var find = items.First(x => x == item);

        if (Managers.Game.Player.Stats.GoldStat.DefaultValue < find.Gold) 
            return false;
        
        Managers.Game.Inventory.AddItem(Managers.Data.SetItemData(find));
        Managers.Game.Player.GainGold(-find.Gold);
        gold += find.Gold;

        return true;
    }
    
    public bool Buy(Item item)
    {
        var find = items.First(x => x == item.ItemData);

        if (Managers.Game.Player.Stats.GoldStat.DefaultValue < find.Gold) 
            return false;
        
        Managers.Game.Inventory.AddItem(Managers.Data.SetItemData(find));
        Managers.Game.Player.GainGold(-find.Gold);
        gold += find.Gold;

        return true;
    }
    

    // 플레이어가 아이템 판매
    public bool Sell(SO_Item item)
    {
        if (gold < item.Gold)
            return false;

        Managers.Game.Inventory.RemoveItem(Managers.Data.SetItemData(item));
        Managers.Game.Player.GainGold(item.Gold);
        gold -= item.Gold;
        
        // TODO : 재구매 항목 만들시 여기 수정 필요
        
        return true;
    }

    public bool Sell(Item item, int amount = 1)
    {
        if (gold < item.ItemData.Gold)
            return false;

        if (item is EquipmentItem)
        {
            if (Managers.Game.Inventory.WeaponSlots.ToList().Find(x => x != (EquipmentItem)item) != null)
                Managers.Game.Inventory.RemoveItem(item);
            else
                return false;
        }
        else
        {
            Managers.Game.Inventory.SubItem((CountableItem)item, amount);
        }
        
        Managers.Game.Player.GainGold(item.ItemData.Gold * amount);
        gold -= item.ItemData.Gold;

        Debug.Log($"판 가격 : {item.ItemData.Gold}");
        // TODO : 재구매 항목 만들시 여기 수정 필요
        
        return true;
    }
}