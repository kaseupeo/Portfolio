using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemQuickSlotUI : SceneUI
{
    private ConsumptionQuickSlot[] quickSlots;
    private InventoryUI invenUI;
    private GameObject go_SelectedImage;
    private RectTransform rect;
    private int selectedSlot;

    private ConsumptionQuickSlot curDragSlot;
    private Sprite dragImage;

    public ConsumptionQuickSlot[] QuickSlots => quickSlots;

    protected override void Awake()
    {
        base.Awake();
        Managers.UI.ItemQuickSlot = this;
        invenUI = Managers.UI.InventoryUI;
        quickSlots = new ConsumptionQuickSlot[(int)ItemQuickSlotType.Slot6 + 1];
        rect = GetComponent<RectTransform>();
        for (int i = 0; i < quickSlots.Length; i++)
        {
            var slot = Managers.Resource.Instantiate<ConsumptionQuickSlot>("UI/Slot/ConsumptionQuickSlot", transform, false);
            slot.name = ($"itemSlot{i}");
            slot.slotType = (ItemQuickSlotType)i;
            // slot.SetItem(null);
            quickSlots[i] = slot;
            slot.Init(Managers.Input.ItemAction[i +1]);
        }
       
    }
    public void UpdateQuickSlot(Consumption[] quickslot, int idx = -1)
    {
        if (idx == -1)
        {
            for (int i = 0; i < quickslot.Length; i++)
            {
                quickSlots[i].SetItem(quickslot[i]);
            }
        }
        else quickSlots[idx].SetItem(quickslot[idx]);
    }

    public void OnBeginDrag(ConsumptionQuickSlot slot)
    {
       dragImage = slot.curItem.Icon;
       
       // dragImage.sprite = slot.curItem.Icon;
       // dragImage.gameObject.SetActive(true);
    }
    public void OnDrag(ConsumptionQuickSlot slot, Vector3 position)
    {
        curDragSlot = slot;
        //dragImage.rectTransform.position = position;
    }
    public void OnEndDrag()
    {
        //if(dragImage == null) dragImage = invenUI.dragimage;
        //curDragSlot = null;
       // dragImage.gameObject.SetActive(false);
    }

    public void Use(Consumption item)
    {
        Managers.Game.Inventory.Use(item);
    }
    public void RemoveItemFromSlot(ConsumptionQuickSlot slot)
    {
        Managers.Game.Inventory.RemoveQuickSlot(slot.slotType);
      
    }



}
