using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class ConsumptionQuickSlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler
{
    public ItemQuickSlotType slotType;
    Inventory invenUI;
    ItemQuickSlotUI quickSlotUI;
    public TextMeshProUGUI amountText;
    public Image itemImage;
    public Sprite defaultSprite;
    public Consumption curItem;
    public int amount;

    private InputAction input;
    
    void Start()
    {
        amountText.gameObject.SetActive(false);
        quickSlotUI = transform.parent.GetComponent<ItemQuickSlotUI>();
    }
    
    void Update()
    {
        if(input.IsPressed())
        {
            if (curItem == null) return;
            quickSlotUI.Use(this.curItem);
            SetItem(curItem);
        }
    }
    public void Init(InputAction useInput)
    {
        input = useInput;
    }
    public void SetItem(Consumption item)
    {
        curItem = item;
        if (item == null)
        {
            itemImage.sprite = defaultSprite;
            itemImage.color = new Color
           (
           itemImage.color.r,
           itemImage.color.g,
           itemImage.color.b,
           1);
            amountText.gameObject.SetActive(false);
            return;
        }
        else
        {
            Debug.Log($"CurItem name {curItem.ItemName}");
            itemImage.sprite = curItem.Icon;
            amount = curItem.Amount;
            amountText.gameObject.SetActive(true);
            amountText.text = curItem.Amount.ToString();
        }
    }

    public void Equip(Consumption consumeItem)
    {
        //Inventory.Instance.RemoveQuickSlot(slotType);
        Managers.Game.Inventory.SetQuickSlot(slotType, consumeItem);
        SetItem(consumeItem);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (curItem == null) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            quickSlotUI.Use(this.curItem);
            SetItem(curItem);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("퀵슬롯 오른쪽 버튼");
            quickSlotUI.RemoveItemFromSlot(this);
            SetItem(null);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (curItem == null) return;
        quickSlotUI.OnDrag(this, eventData.position);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (curItem == null) return;
        quickSlotUI.OnBeginDrag(this);

    }
    public void OnDrop(PointerEventData eventData)
    {

        if (eventData.pointerDrag.TryGetComponent(out ItemSlot nextSlot))
        {
            Debug.Log($"퀵슬롯 OnDrop");
            if (nextSlot.CurItem is Consumption consumeItem)
            {
                Equip(consumeItem);
            }

            //Inventory.Instance.RemoveQuickSlot(slotType);
            //SetItem(null);

        }
        //else if(eventData.pointerDrag.TryGetComponent(out ConsumptionQuickSlot quickSlot))
        //{
        //    if (nextSlot.CurItem is Consumption consumeItem)
        //    {
        //        WeaponEquip(consumeItem);
        //    }
        //}
        quickSlotUI.OnEndDrag();
    }

}
