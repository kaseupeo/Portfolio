using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public enum InventoryType { Equipment, Consume, Etc, }
public enum ItemQuickSlotType { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6 };

public class Inventory : MonoBehaviour
{
    private EquipmentItem[] equipmentInv;
    private Consumption[] consumeptionInv;
    private ETC[] etcInv;
    private Consumption[] quickSlot;
    private EquipmentItem[] weaponSlots;
    private InventoryUI invenUI;
    private ItemQuickSlotUI itemQuickSlotUI;
    private int maxCapacity = 54;

    public Creature Owner { get; set; }

    public EquipmentItem[] EquipmentInv => equipmentInv;
    public Consumption[] ConsumeptionInv => consumeptionInv;
    public ETC[] EtcInv => etcInv;
    public Consumption[] QuickSlot => quickSlot;
    public EquipmentItem[] WeaponSlots => weaponSlots;
    public int MaxCapacity => maxCapacity;

    public void Init()
    {
        Managers.Game.Inventory = this;
        Owner = Managers.Game.Player;
        itemQuickSlotUI = Managers.UI.ItemQuickSlot;
        invenUI = GetInventoryUI();
        
        equipmentInv = new EquipmentItem[MaxCapacity];
        consumeptionInv = new Consumption[MaxCapacity];
        etcInv = new ETC[MaxCapacity];
        quickSlot = new Consumption[(int)ItemQuickSlotType.Slot6 + 1];
        weaponSlots = new EquipmentItem[(int)WeaponType.Stamp + 1];
    }

    public InventoryUI GetInventoryUI()
    {
        if (invenUI == null)
            invenUI = Managers.UI.InventoryUI;
        
        return invenUI;
    }

    public Item[] GetInvenByInventoryType(InventoryType invenType)
    {
        switch (invenType)
        {
            case InventoryType.Equipment:
                return equipmentInv;

            case InventoryType.Consume:
                return consumeptionInv;

            case InventoryType.Etc:
            default:
                return etcInv;
        }
    }

    public Item[] GetInvenByItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Equipment:
                return equipmentInv;

            case ItemType.Consume:
                return consumeptionInv;

            case ItemType.Etc:
            default:
                return etcInv;
        }
    }

    ///인벤토리에 저장이 가능한지 검사
    private bool IsValidIndex(int idx)
    {
        return idx >= 0 && idx < MaxCapacity;
    }

    public void FindItem(Item item, out int idx)
    {
        Item[] inv = GetInvenByItemType(item.Type);
        idx = -1;
        int i = -1;
        while (++i < MaxCapacity)
        {
            if (inv[i] == item)
            {
                idx = i;
                return;
            }
        }
    }

    ///<summary> 인벤토리 인덱스에 빈 슬롯이 있는지 탐색 </summary>
    private int FindEmptySlotIndex(Item[] inv, int startIdx = 0)
    {
        for (int i = startIdx; i < MaxCapacity; i++)
        {
            if (inv[i] == null) 
                return i;
        }
        
        return -1;
    }

    private (int, bool) FindVacancyForCountableItem(CountableItem[] inv, CountableItem ci, int idx = 0)
    {
        for (int i = idx; i < MaxCapacity; i++)
        {
            var item = inv[i];

            if (item != null && item.ItemData.CodeName == ci.ItemData.CodeName)
            {
                Debug.Log($"아이템 코드네임{item.ItemData.CodeName}, ci 코드네임 {ci.ItemData.CodeName}");
                if (false == item.IsMax)
                    return (i, true);
                
                break;
            }
        }

        for (int i = idx; i < MaxCapacity; i++)
        {
            var item = inv[i];
            if (item == null) 
                return (i, false);
        }
        
        return (-1, false);
    }

    ///<summary> 빈 슬롯 없이 아이템 채우기 </summary>
    public void TrimAll(InventoryType invenType)
    {
        // 가장 빠른 배열 빈공간 채우기 알고리즘

        // i 커서와 j 커서
        // i 커서 : 가장 앞에 있는 빈칸을 찾는 커서
        // j 커서 : i 커서 위치에서부터 뒤로 이동하며 기존재 아이템을 찾는 커서

        // i커서가 빈칸을 찾으면 j 커서는 i+1 위치부터 탐색
        // j커서가 아이템을 찾으면 아이템을 옮기고, i 커서는 i+1 위치로 이동
        // j커서가 Capacity에 도달하면 루프 즉시 종료

        Item[] inv = GetInvenByInventoryType(invenType);
        
        // 1. Trim
        int i = -1;
        while (inv[++i] != null) ;
        int j = i;

        while (true)
        {
            while (++j < MaxCapacity && inv[j] == null) ;

            if (j == MaxCapacity)
                break;

            (inv[j], inv[i]) = (inv[i], inv[j]);
            i++;
        }
        
        // 2. Sort
        Array.Sort(inv, (a, b) =>
        {
            if (a == null)
                return b == null ? 0 : 1;

            if (b == null) 
                return -1;
                
            return String.CompareOrdinal(a.ItemData.CodeName, b.ItemData.CodeName);
        });
        
        // 3. Update
        GetInventoryUI();
        invenUI.UpdateSlotFilterState(); // 필터 상태 업데이트
    }

    //아이템 저장을 시도하는 함수
    public bool AddItem(Item item)
    {
        Item[] inv;

        switch (item.ItemData.Type)
        {
            case ItemType.Equipment:
                inv = equipmentInv;
                break;
            default:
                return AddCountableItem((CountableItem)item);
        }
        
        int idx = FindEmptySlotIndex(inv);
        
        if (idx == -1)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }
        
        inv[idx] = item;
        
        return true;
    }

    public bool AddCountableItem(CountableItem item)
    {
        CountableItem[] inv;
        switch (item.ItemData.Type)
        {
            case ItemType.Consume:
                inv = consumeptionInv;
                break;
            case ItemType.Etc:
            default:
                inv = etcInv;
                break;
                //inv = null;
                //Debug.Log("인벤토리가 Null입니다.");
                //break;
        }
        
        //int idx = FindVacancyForCountableItem(inv, item);
        var test = FindVacancyForCountableItem(inv, item);
        
        if (test.Item1 == -1)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }
        
        if (false == test.Item2) 
            inv[test.Item1] = item;
        else 
            inv[test.Item1].AddAmount(item.Amount);
        
        return true;
    }

    public void RemoveItem(Item item)
    {
        FindItem(item, out int idx);
        var inv = GetInvenByItemType(item.Type);
        if (idx == -1)
        {
            Debug.Log("아이템이 인벤토리에 없음");
            return;
        }
        inv[idx] = null;
        invenUI.UpdateSlotFilterState();
    }
    
    public void SwapItem(Item itemA, Item itemB)
    {
        var inv = GetInvenByItemType(itemA.Type);
        FindItem(itemA, out int idx1);
        FindItem(itemB, out int idx2);

        if (idx1 == -1)
        {
            Debug.Log("잘못된 변경입니다.");
            return;
        }

        inv[idx1] = itemB;
        inv[idx2] = itemA;
        invenUI.UpdateSlotFilterState();
    }
    
    public void SwapItem(InventoryType invenType, int idx1, int idx2)
    {
        var inv = GetInvenByInventoryType(invenType);

        (inv[idx1], inv[idx2]) = (inv[idx2], inv[idx1]);
        invenUI.UpdateSlotFilterState();
    }

    /// <summary> 셀 수 있는 아이템의 수량 나누기(A -> B 슬롯으로) </summary>
    public void SeparateAmount(Item item, int indexA, int indexB, int amount)
    {
        CountableItem ItemA = item as CountableItem;
        var inv = GetInvenByItemType(item.Type);
        if (inv == equipmentInv)
        {
            Debug.Log("나눌 수 없는 아이템을 나누려고 합니다.");
            return;
        }
        
        // amount : 나눌 목표 수량
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;
        
        ItemA = (CountableItem)inv[indexA];
        CountableItem itemB = (CountableItem)inv[indexB];
        
        // 조건 : A 슬롯 - 셀 수 있는 아이템 / B 슬롯 - Null
        // 조건에 맞는 경우, 복제하여 슬롯 B에 추가
        if (ItemA != null && itemB == null)
        {
            inv[indexB] = ItemA.SeperateAndClone(amount);

            invenUI.UpdateInvenSlot(inv, indexA);
            invenUI.UpdateInvenSlot(inv, indexB);
        }
    }

    /// <summary> 해당 슬롯의 아이템 사용 </summary>
    public void Use(Item item)
    {
        FindItem(item, out int idx);
        var inv = GetInvenByItemType(item.Type);
        if (!IsValidIndex(idx)) return;
        if (inv[idx] == null) return;

        // 사용 가능한 아이템인 경우
        // 아이템 사용
        bool succeeded = inv[idx].Use();

        if (succeeded)
        {
            Debug.Log($"아이템 사용 succeded : {succeeded}, {inv[idx].ItemName}");
            invenUI.UpdateInvenSlot(inv, idx);
        }
    }

    public bool SubItem(CountableItem ci, int amount)
    {
        if (ci.Amount < amount && false == ci.IsEmpty) return false;
        ci.SubAmount(amount);
        if (ci.IsEmpty)
        {
            RemoveItem(ci);
        }
        else invenUI.UpdateSlotFilterState();
        return true;
    }
    
    #region QuickSlot
    
    public Consumption[] GetQuickSlot()
    {
        return quickSlot;
    }

    public Consumption GetQuickSlot(ItemQuickSlotType type)
    {
        return quickSlot[(int)type];
    }

    public void SetQuickSlot(ItemQuickSlotType type, Consumption item)
    {
        Debug.Log($"퀵슬롯 이름 {item.ItemName}");
        quickSlot[(int)type] = item;
    }
    
    public void RemoveQuickSlot(ItemQuickSlotType type)
    {
        quickSlot[(int)type] = null;
        itemQuickSlotUI.UpdateQuickSlot(GetQuickSlot());
    }

    public bool InitQuickSlot(ItemQuickSlotType type)
    {
        int idx = (int)type;
        Consumption consumeItem = quickSlot[idx];
        
        if (consumeItem == null)
        {
            return true;
        }

        quickSlot[idx] = null;
        bool result = AddItem(consumeItem);
        
        if (result == false)
        {
            quickSlot[idx] = consumeItem;
            return false;
        }
        
        TrimAll(InventoryType.Consume);
        return true;
    }
    
    #endregion

    #region WeaponSlot

    public EquipmentItem[] GetWeaponSlot()
    {
        return WeaponSlots;
    }

    public bool Equip(EquipmentItem item, WeaponSlot slot = null)
    {
        if (slot == null)
        {
            if (weaponSlots.Length <= 0) 
                return false;
            
            Managers.Game.OnWeaponEquiped?.Invoke(item.SO_equipment);
            weaponSlots[(int)item.weaponType] = item;

            return true;
        }

        if (item.weaponType != slot.weaponType) 
            return false;
        
        weaponSlots[(int)item.weaponType] = item;
        Managers.Game.Player.EquipmentSystem.ExchangedWeapon(item.SO_equipment);
        invenUI.UpdateSlotFilterState();
            
        return true;
    }
    
    #endregion
}
