using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ItemDragHandler : DragHandler
{
    public Item Item { get; private set; }
    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        image.raycastTarget = false;
    }
    void Start()
    {

    }

    void Update()
    {

    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("비긴 드래그");
        Item = GetComponentInParent<ItemSlot>().CurItem;
        if (Item == null)
            return;

        Managers.UI.DragHandler = this;
        isPointerOverUI = true;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        image.raycastTarget = false;
        image.sprite = Skill.Icon;
        image.color = new Color(1, 1, 1, 0.5f);
    }

    public override void OnDrag(PointerEventData eventData)
    {
       
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        
    }



}
