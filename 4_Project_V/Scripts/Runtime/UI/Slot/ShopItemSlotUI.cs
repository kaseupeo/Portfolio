using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemSlotUI : ItemSlotUI, IPointerClickHandler
{
    public override void SellOrBuy()
    {
        base.SellOrBuy();
        if (Managers.Game.Shop.Buy(Item))
        {
            Managers.UI.ShopPopupUI.UpdateSlotList();
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