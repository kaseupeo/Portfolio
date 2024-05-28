using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPopupUI : PopupUI
{
    public GameObject text;
    Vector2 offset = new Vector2(2, 0f);
    Button equipButton;
    Item item;
    RectTransform rect;
    InventoryUI invenUI;

    Image itemImage;
    TMP_Text itemNameText;
    TMP_Text itemInfoText;


    protected override void Awake()
    {
        base.Awake();
        itemImage = Images["ItemIconImage"];
        itemNameText = Texts["ItemNameText"];
        itemInfoText = Texts["ItemInfoText"];

        buttons["EquipButton"].onClick.AddListener(Equip);
        buttons["DropButton"].onClick.AddListener(DropItem);
        equipButton = buttons["EquipButton"];
        equipButton.onClick.AddListener(Equip);
        rect = GetComponent<RectTransform>();
    }
    private void Start()
    {
        invenUI = Managers.UI.InventoryUI;
       
    }
    public void SetItem(Item item, Vector2 position)
    {
        this.item = item;
        rect.position = position + offset;
        switch (item.Type)
        {
            case Define.ItemType.Equipment:
                equipButton.gameObject.SetActive(true);
                break;
            case Define.ItemType.Consume:
                equipButton.gameObject.SetActive(true);
                break;
            case Define.ItemType.Etc:
                equipButton.gameObject.SetActive(false);
                break;
            default:
                equipButton.gameObject.SetActive(true);
                break;
        }
        SetDescription(item);
    }

    public void SetDescription(Item item)
    {
        itemImage.sprite = item.Icon;
        itemNameText.text = item.ItemName;
        itemInfoText.text = item.ItemData.Description;
    }

    private void Equip()
    {
        invenUI.GetEquipByInfoPanel(item);
        CloseUI();
    }

    private void DropItem()
    {
        CloseUI();
        if (item != null)
        {
            Managers.Game.Inventory.RemoveItem(item);
            invenUI.UpdateSlotFilterState();
        }

    }

}
