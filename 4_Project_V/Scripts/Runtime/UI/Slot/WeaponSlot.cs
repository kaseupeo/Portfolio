using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
{
    public WeaponType weaponType;
    private InventoryUI invenUI;
    [SerializeField] private Image[] itemImages = new Image[2];
    private Sprite[] defaultSprites = new Sprite[2];
    private EquipmentItem curItem;
    private RectTransform rect;

    public EquipmentItem CurItem => curItem;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        defaultSprites[0] = itemImages[0].sprite;
        defaultSprites[1] = itemImages[1].sprite;

    }
    private void Start()
    {
        invenUI = Managers.UI.InventoryUI;
        itemImages[0].raycastTarget = false;
        itemImages[1].raycastTarget = false;
    }
    private void SetAlpha(float alpha)
    {
        itemImages[1].color = new Color
            (
            itemImages[1].color.r,
            itemImages[1].color.g,
            itemImages[1].color.b,
            alpha
            );
        //Color color;
        //color = itemImages[1].color;
        //color.a = alpha;
    }
    private void SetdefaultImage()
    {
        itemImages[0].sprite = defaultSprites[0];
        itemImages[1].sprite = defaultSprites[1];
        SetAlpha(0);
    }

    private void SetIcon(EquipmentItem item)
    {
        var itemData = item.ItemData;
        itemImages[0].sprite = itemData.Rarity.Rarityicon;
        itemImages[1].sprite = itemData.Icon;
        SetAlpha(1);
    }
    public void SetItem(EquipmentItem item)
    {
        curItem = item;
        if (item != null)
        {
            if (item.Type == Define.ItemType.Equipment)
                SetIcon(item);
            return;
        }
        else
        {
            SetdefaultImage();
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (curItem != null) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            eventData.pointerClick.TryGetComponent(out WeaponSlot slot);
            Managers.UI.ShowPopupUI<ItemInfoPopupUI>("UI/Popup/ItemInfoPanel").SetItem(slot.curItem, rect.position);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (curItem == null) return;


    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (curItem == null) return;

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (curItem == null) return;


    }
    public void OnEndDrag(PointerEventData eventData)
    {

    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out ItemSlot slot))
        {
            if (slot.CurItem.Type == Define.ItemType.Equipment && slot.CurItem is EquipmentItem equip)
            {
                if (this.weaponType == equip.weaponType)
                {
                    Managers.Game.Inventory.Equip(equip, this);
                    //slot.IsEquiped = true;
                }
            }
        }
        else
            Debug.Log("잘못된 타입의 장비를 착용하려고 합니다");
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

}
