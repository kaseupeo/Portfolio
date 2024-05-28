using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerItemSlotUI : ItemSlotUI, IPointerClickHandler
{
    public override void SellOrBuy()
    {
        base.SellOrBuy();
        if (Managers.Game.Shop.Sell(Item, 1))
        {
            if (Item is not EquipmentItem && ((CountableItem)Item).Amount != 0)
                UpdateSlot();
            else
                Clear();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Item == null)
            return;

        if (Time.time - _doubleClickedTime < _interval)
        {
            _isDoubleClicked = true;
            _doubleClickedTime = -1.0f;

            SellOrBuy();
        }
        else
        {
            _isDoubleClicked = false;
            _doubleClickedTime = Time.time;
        }
    }
}