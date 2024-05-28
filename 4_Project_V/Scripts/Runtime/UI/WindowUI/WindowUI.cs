using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowUI : BaseUI, IDragHandler, IPointerDownHandler
{

    protected override void Awake()
    {
        base.Awake();
        //buttons["CloseButton"].onClick.AddListener(() => {Managers.UI.CloseWindowUI(this); });
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       // GameManager.UI.SelecteWindow(this);
    }
}