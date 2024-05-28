using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ItemSlotUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image rarity;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI count;
    
    private Item _item;

    public Item Item => _item;

    public void Init(Item item)
    {
        if (item == null)
            return;

        _item = item;
        
        rarity.sprite = item.ItemData.Rarity.Rarityicon;
        itemIcon.sprite = item.Icon;
        itemIcon.color = new Color(1, 1, 1, 1);
        if (item is CountableItem)
            count.text = ((CountableItem)item).Amount.ToString();
    }

    public void UpdateSlot()
    {
        count.text = ((CountableItem)_item).Amount.ToString();
    }
    
    public void Clear()
    {
        _item = null;
        rarity.sprite = null;
        itemIcon.sprite = null;
        itemIcon.color = new Color(1, 1, 1, 0);
        count.text = "";
    }

    public virtual void SellOrBuy()
    {
    }
    
    protected float _interval = 0.25f;
    protected float _doubleClickedTime = -1.0f;
    protected bool _isDoubleClicked;
    

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}