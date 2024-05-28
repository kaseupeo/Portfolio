using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotDragHandler : DragHandler
{
    private void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        Skill = GetComponentInParent<SkillSlotUI>().Skill;

        if (Skill == null)
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
        isPointerOverUI = true;
        transform.position = eventData.position;    
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        Managers.UI.DragHandler = null;
        isPointerOverUI = false;

        transform.localPosition = Vector3.zero;
        GetComponentInParent<SkillSlotUI>().Skill = IsSucceedDrop ? Skill : null;
        IsSucceedDrop = false;
        image.raycastTarget = true;
        image.color = new Color(1, 1, 1, 0);
    }
}