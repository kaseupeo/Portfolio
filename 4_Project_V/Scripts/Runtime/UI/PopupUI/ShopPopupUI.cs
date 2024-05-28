using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopPopupUI : PopupUI, IPointerEnterHandler, IPointerExitHandler
{
    [Header("항목")]
    
    
    [Header("상점")]
    [SerializeField] private RectTransform shopContent;
    [SerializeField] private GameObject shopItemPrefab;

    [Header("플레이어 가방")] 
    [SerializeField] private RectTransform inventoryContent;
    [SerializeField] private GameObject inventoryItemPrefab;

    private InventoryType _type;
    private List<GameObject> _inventorySlotList = new();
    private List<GameObject> _shopSlotList = new();
    
    private bool _isPointerOverUI;
    public bool IsPointerOverUI => _isPointerOverUI;

    protected override void Awake()
    {
        base.Awake();
        Managers.UI.ShopPopupUI = this;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        texts["ShopGoldAmount"].text = $"{Managers.Game.Shop.Gold}";
        texts["GoldAmount"].text = $"{Managers.Game.Player.Stats.GoldStat.DefaultValue}";

        CreateSlot();
        BindButton();
        UpdateShop();
        UpdateSlotList();
        gameObject.SetActive(false);
        
        Managers.Game.Player.OnGainGold += UpdateGold;
        Managers.Game.Inventory.GetInventoryUI();
    }

    private void CreateSlot()
    {
        for (int i = 0; i < 54; i++)
        {
            var inventorySlot = Managers.Resource.Instantiate(inventoryItemPrefab, inventoryContent);
            _inventorySlotList.Add(inventorySlot);
            var shopSlot = Managers.Resource.Instantiate(shopItemPrefab, shopContent);
            _shopSlotList.Add(shopSlot);
        }
    }

    private void BindButton()
    {
        buttons["EquipButton"].onClick.AddListener(() =>
        {
            _type = InventoryType.Equipment;
            UpdateSlotList();
        });
        buttons["ConsumeButton"].onClick.AddListener(() =>
        {
            _type = InventoryType.Consume;
            UpdateSlotList();
        });
        buttons["EtcButton"].onClick.AddListener(() =>
        {
            _type = InventoryType.Etc;
            UpdateSlotList();
        });
    }
    
    private void UpdateShop()
    {
        for (int i = 0; i < _shopSlotList.Count; i++)
        {
            var slot = _shopSlotList[i].GetComponent<ItemSlotUI>();
            if (Managers.Game.Shop.Items.Count > i)
            {
                var item = Managers.Game.Shop.Items[i];
                UpdateSlot(slot, Managers.Data.SetItemData(item));
            }
        }
    }
    
    private void UpdateGold(Creature creature, float gold)
    {
        texts["GoldAmount"].text = $"{gold}";
    }
    
    public void UpdateSlotList()
    {
        for (int i = 0; i < _inventorySlotList.Count; i++)
        {
            var slot = _inventorySlotList[i].GetComponent<ItemSlotUI>();
            slot.Clear();
            Item item = null;
            switch (_type)
            {
                case InventoryType.Equipment:
                    item = Managers.Game.Inventory.EquipmentInv[i];
                    break;
                case InventoryType.Consume:
                    item = Managers.Game.Inventory.ConsumeptionInv[i];
                    break;
                case InventoryType.Etc:
                    item = Managers.Game.Inventory.EtcInv[i];
                    break;
            }
            
            UpdateSlot(slot, item);
        }
    }
    
    private void UpdateSlot(ItemSlotUI slotUI, Item item)
    {
        slotUI.Init(item);
    }
    
    public void Show(Creature creature)
    {
        gameObject.SetActive(true);
        UpdateSlotList();
    }

    public void Hide()
    {
        _isPointerOverUI = false;
        gameObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;
    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;
}