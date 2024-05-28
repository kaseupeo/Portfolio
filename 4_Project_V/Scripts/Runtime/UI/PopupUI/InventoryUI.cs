using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class InventoryUI : PopupUI, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private Transform targetTransform; // 이동될 UI

    private Vector2 beginPoint;
    private Vector2 moveBegin;

    public InventoryType curFilterType;

    private Item[] inv;
    [HideInInspector] public ItemSlot[] itemSlots;
    [SerializeField] private WeaponSlot[] weaponSlots;
    private WeaponSlot curWeaponSlot;
    private ItemSlot curSlot;
    private bool _isPointerOverUI;

    public RectTransform itemContent;
    public ItemSlot CurDragSlot;
    public Image dragimage;
    private int dragidx;

    public WeaponSlot[] WeaponSlots => weaponSlots;
    public WeaponSlot CurWeaponSlot => curWeaponSlot;
    public bool IsPointerOverUI => _isPointerOverUI;

    protected override void Awake()
    {
        base.Awake();
        Managers.UI.InventoryUI = this;
       
        // 이동 대상 UI를 지정하지 않은 경우, 자동으로 부모로 초기화
        if (targetTransform == null)
            targetTransform = transform.parent;
    }

    private void Start()
    {
        Init();
    }
    
    public void Init()
    {
        texts["GoldAmount"].text = $"{Managers.Game.Player.Stats.GoldStat.DefaultValue}";
        itemSlots = new ItemSlot[Managers.Game.Inventory.MaxCapacity];
        weaponSlots = new WeaponSlot[(int)WeaponType.Stamp + 1];
        itemContent = transforms["InvenContent"].GetComponent<RectTransform>();
        dragimage = images["DragImage"];
        dragimage.gameObject.SetActive(false);

        Managers.Game.Player.OnGainGold += UpdateGold;

        buttons["EquipButton"].onClick.AddListener(() =>
        {
            curFilterType = InventoryType.Equipment;
            SortInven();
        });

        buttons["ConsumeButton"].onClick.AddListener(() =>
        {
            curFilterType = InventoryType.Consume;
            SortInven();
        });

        buttons["EtcButton"].onClick.AddListener(() =>
        {
            curFilterType = InventoryType.Etc;
            SortInven();
        });

        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slot = Managers.Resource.Instantiate<ItemSlot>("UI/Slot/ItemSlot", itemContent.transform, false);
            slot.name = ($"itemSlot{i}");
            itemSlots[i] = slot;
            slot.Init(this, i);
            
        }        
        
        weaponSlots[0] = transforms["SwordSlot"].GetComponent<WeaponSlot>();
        weaponSlots[0].weaponType = WeaponType.Sword;
        weaponSlots[1] = transforms["BowSlot"].GetComponent<WeaponSlot>();
        weaponSlots[1].weaponType = WeaponType.Bow;
        weaponSlots[2] = transforms["StampSlot"].GetComponent<WeaponSlot>();
        weaponSlots[2].weaponType = WeaponType.Stamp;
        
        gameObject.SetActive(false);
    }
   

    private void UpdateGold(Creature creature, float gold)
    {
        texts["GoldAmount"].text = $"{gold}";
    }

    public void Show(Creature creature)
    {
        gameObject.SetActive(true);
        curFilterType = InventoryType.Equipment;
        SortInven();
    }
    public void Hide()
    {
        _isPointerOverUI = false;
        gameObject.SetActive(false);
    }

    public void HideItemAmountText(int idx)
    {
        if (itemSlots[idx] == null) return;
        itemSlots[idx].HideAmountText();
    }

    public void UpdateSlotFilterState()
    {
        switch (curFilterType)
        {
            case InventoryType.Equipment:
                inv = Managers.Game.Inventory.EquipmentInv;
                break;

            case InventoryType.Consume:
                inv = Managers.Game.Inventory.ConsumeptionInv;
                break;
            case InventoryType.Etc:
                inv = Managers.Game.Inventory.EtcInv;
                break;
        }
        UpdateInvenSlot(inv);
        if (curFilterType == InventoryType.Equipment)
        {
            UpdateWeaponSlot();
            UpdateEquipStates();
        }
    }
    public void RemoveItem(int idx)
    {
        itemSlots[idx].ResetSlot();
    }


    /// <summary>
    /// 인벤토리 UI 업데이트. Inventory에서 실행할 것
    /// </summary>
    /// <param name="inv"></param>
    /// <param name="idx"></param>
    public void UpdateInvenSlot(Item[] inv, int idx = -1)
    {
        if (idx == -1)
        {
            for (int i = 0; i < inv.Length; i++)
            {
                itemSlots[i].SetItem(inv[i]);
            }
        }
        else itemSlots[idx].SetItem(inv[idx]);

    }

    public void UpdateWeaponSlot(int idx = -1)
    {
        var slots = Managers.Game.Inventory.GetWeaponSlot();
        if (idx == -1)
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                weaponSlots[i].SetItem(slots[i]);
            }
        }
    }
    
    public void UpdateEquipStates()
    {
        int j = -1;
        while (++j < weaponSlots.Length)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (weaponSlots[j].CurItem != null && itemSlots[i].CurItem == weaponSlots[j].CurItem)
                {
                    itemSlots[i].IsEquiped = true;
                }
                else if (itemSlots[i].CurItem != weaponSlots[j].CurItem)
                {
                    itemSlots[i].IsEquiped = false;
                }
            }
        }
    }

    public void ClickSlot(ItemSlot slot)
    {
        curSlot = slot;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (curSlot == itemSlots[i])
            {
                dragidx = i;
                Debug.Log($" 클릭된 슬롯 인덱스 값{i}");
                return;
            }
        }
    }
    public void BeginDrag(ItemSlot slot)
    {
        CurDragSlot = slot;
        Debug.Log($"드래그 슬롯 인덱스{CurDragSlot.SlotIdx}");
        dragimage.gameObject.SetActive(true);
        dragimage.sprite = slot.ItemImage[1].sprite;
    }
    public void SlotOnDrag(ItemSlot slot, Vector3 position)
    {
        CurDragSlot = slot;
        dragimage.rectTransform.position = position;
    }
    public void ExitDrag()
    {
        //CurDragSlot = null;
        //dragimage.sprite = null;
        dragimage.gameObject.SetActive(false);
    }

    public void SortInven()
    {
        Managers.Game.Inventory.TrimAll(curFilterType);
    }

    public bool GetEquipByInfoPanel(Item item)
    {
        switch (item.Type)
        {
            case Define.ItemType.Equipment:
                WeaponEquip((EquipmentItem)item);
                break;
            case Define.ItemType.Consume:
                ConsumptionEquip((Consumption)item);
                break;
            default:
                return false;
        }

        return true;
    }

    public void ConsumptionEquip(Consumption consumeItem)
    {
        int emptySlotIdx = -1;
        var slots = Managers.Game.Inventory.GetQuickSlot();
        for (int i = 0; i < (int)ItemQuickSlotType.Slot5 + 1; i++)
        {
            if (slots[i] == null)
            {
                emptySlotIdx = i;
                break;
            }
        }
        if (emptySlotIdx == -1)
        {
            emptySlotIdx = 0;
        }
        Managers.Game.Inventory.SetQuickSlot((ItemQuickSlotType)emptySlotIdx, consumeItem);
    }

    public void WeaponEquip(EquipmentItem item)
    {
        Managers.Game.Inventory.Equip(item);
    }

    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;
    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;
    // 드래그 시작 위치 지정
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        beginPoint = targetTransform.position;
        moveBegin = eventData.position;
    }

    // 드래그 : 마우스 커서 위치로 이동
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        targetTransform.position = beginPoint + (eventData.position - moveBegin);
    }

   
}