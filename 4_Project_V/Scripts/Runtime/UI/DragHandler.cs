using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected Image image;
    protected RectTransform rectTransform;
    
    protected bool isPointerOverUI;
    public bool IsPointerOverUI => isPointerOverUI;
    public Skill Skill { get; set; }
    public bool IsSucceedDrop { get; set; }

    public abstract void OnBeginDrag(PointerEventData eventData);
    public abstract void OnDrag(PointerEventData eventData);
    public abstract void OnEndDrag(PointerEventData eventData);
}