using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private Image image;
    private Item _item;
    private float _delayedSecond = 0.1f;

    public Item Item { get => _item; set => _item = value; }

    public void SetItem(Item item)
    {
        if (_item != null)
            return;
        
        image.sprite = item.Sprite;
        _item = item;
        image.color = new Color(1, 1, 1, 1);
    }

    public void ResetSlot()
    {
        image.color = new Color(1, 1, 1, 0);
        image.sprite = null;
        _item = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_item == null)
            return;
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Potion potion = _item as Potion;
            potion.Use();
            ResetSlot();
        }
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GetComponentInParent<Inventory>().ToolTip.gameObject.SetActive(true);
            GetComponentInParent<Inventory>().ToolTip.gameObject.transform.position = transform.position ;
            var text = GetComponentInParent<Inventory>().ToolTip.GetComponentInChildren<TextMeshProUGUI>(); 
            text.text = $"{_item.Name}\n\n{_item.Description}";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData != null)
        {
            GetComponentInParent<Inventory>().ToolTip.gameObject.SetActive(false);
        }
    }
}
