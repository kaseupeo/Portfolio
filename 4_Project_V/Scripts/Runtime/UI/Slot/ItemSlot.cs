using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler,
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler, IPointerExitHandler
{

    public RectTransform rect;
    /// <summary>
    /// slot은 Inventory에서 Item의 이미지를 받아옴
    /// 0번째 = Rarity Icon / 1번째 = item Icon
    /// </summary>
    [SerializeField] private Image[] itemImage = new Image[2];
    [SerializeField] private TextMeshProUGUI text_Count;
    [SerializeField] private Sprite[] defaultSprites;
    [SerializeField] private GameObject highlightGo;
    [SerializeField] private Image equipedImage;
    [Space]
    [Tooltip("하이라이트 이미지 알파 값")]
    [SerializeField] private float highlightAlpha = 0.5f;
    [Tooltip("하이라이트 소요 시간")]
    [SerializeField] private float highlightFadeDuration = 0.2f;
    // 현재 하이라이트 알파값
    private float currentHLAlpha = 0f;
    [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
    [SerializeField] private Image highlightImage;
    private GameObject textObj => text_Count.gameObject;

    private InventoryUI invenUI;
    private Item curItem;
    private int slotIdx;
    public int SlotIdx => slotIdx;

    private bool isEquiped;
    public bool IsEquiped
    {
        get => isEquiped;
        set
        {
            if (isEquiped == value) return;
            isEquiped = value;
            equipedImage.gameObject.SetActive(value);
        }
    }

    public Image[] ItemImage => itemImage;
    public Item CurItem => curItem;

    private void Awake()
    {
        InitComponents();
        InitValues();
    }
    
    private void InitComponents()
    {
        // Game Objects
        rect = GetComponent<RectTransform>();
        highlightGo = highlightImage.gameObject;
    }
    private void InitValues()
    {
        defaultSprites = new Sprite[2];
        defaultSprites[0] = ItemImage[0].sprite;
        defaultSprites[1] = ItemImage[1].sprite;

       // itemImage[0].raycastTarget = false;
       // itemImage[1].raycastTarget = false;
        highlightImage.raycastTarget = false;
        equipedImage.raycastTarget = false;

        // 2. Deactivate Icon
        ResetSlot();
        highlightGo.SetActive(false);
        equipedImage.gameObject.SetActive(false);
    }


    private void ShowSlot(bool value) => gameObject.SetActive(value);
    private void HideSlot(bool value) => gameObject.SetActive(value);

    public void Init(InventoryUI invenUI, int slotIdx)
    {
        this.invenUI = invenUI;
        this.slotIdx = slotIdx;
    }
    private void SetAlpha(float alpha)
    {
        itemImage[1].color = new Color
            (
            itemImage[1].color.r,
            itemImage[1].color.g,
            itemImage[1].color.b,
            alpha
            );
        //Color color;
        //color = itemImages[1].color;
        //color.a = alpha;
    }
    private void SetdefaultImage()
    {
        itemImage[0].sprite = defaultSprites[0];
        itemImage[1].sprite = defaultSprites[1];
        SetAlpha(0);
    }

    private void SetIcon(Item item)
    {
        var itemData = item.ItemData;
        itemImage[0].sprite = itemData.Rarity.Rarityicon;
        itemImage[1].sprite = itemData.Icon;
        SetAlpha(1);
    }

    // 해당 슬롯의 아이템 갯수 업데이트
    public void DisplayAmountText(CountableItem item)
    {
        if (item.Amount == 1) return;
        textObj.SetActive(true);
        text_Count.text = item.Amount.ToString();

    }
    public void HideAmountText()
    {
        //text_Count.message.Remove(text_Count.message.Length - 1);
        textObj.SetActive(false);
    }

    // 슬롯 초기화
    public void SetItem(Item item)
    {
        curItem = item;
        if (item != null)
        {
            if (item.Type != Define.ItemType.Equipment && item is CountableItem ci)
            {
                DisplayAmountText(ci);
            }
            else
            {
                HideAmountText();
            }
            SetIcon(item);
            return;
        }
        else
        {
            ResetSlot();
        }
    }
    public void ResetSlot()
    {
        SetdefaultImage();
        HideAmountText();
    }
    /// <summary> 슬롯에 하이라이트 표시/해제 </summary>
    public void Highlight(bool show)
    {
        if (show)
            StartCoroutine(nameof(HighlightFadeInRoutine));
        else
            StartCoroutine(nameof(HighlightFadeOutRoutine));
    }
    /// <summary> 하이라이트 알파값 서서히 증가 </summary>
    private IEnumerator HighlightFadeInRoutine()
    {
        StopCoroutine(nameof(HighlightFadeOutRoutine));
        highlightGo.SetActive(true);

        float unit = highlightAlpha / highlightFadeDuration;

        for (; currentHLAlpha <= highlightAlpha; currentHLAlpha += unit * Time.deltaTime)
        {
            highlightImage.color = new Color(
                highlightImage.color.r,
                highlightImage.color.g,
                highlightImage.color.b,
                currentHLAlpha
            );

            yield return null;
        }
    }

    /// <summary> 하이라이트 알파값 0%까지 서서히 감소 </summary>
    private IEnumerator HighlightFadeOutRoutine()
    {
        StopCoroutine(nameof(HighlightFadeInRoutine));

        float unit = highlightAlpha / highlightFadeDuration;

        for (; currentHLAlpha >= 0f; currentHLAlpha -= unit * Time.deltaTime)
        {
            highlightImage.color = new Color(
                highlightImage.color.r,
                highlightImage.color.g,
                highlightImage.color.b,
                currentHLAlpha
            );

            yield return null;
        }

        highlightGo.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("클릭");
        if (curItem == null) return;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($" 인포패널 {this.slotIdx}");
            invenUI.ClickSlot(this);
            Managers.UI.OpenItemInfoUI(curItem, eventData.position);
        }
        //if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    Inventory.Instance.
        //}

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (curItem == null) return;
        Highlight(true);
        Managers.UI.OpenItemInfoUI(curItem, eventData.position);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (curItem == null) return;
        invenUI.BeginDrag(this);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (curItem == null) return;
        invenUI.SlotOnDrag(this, eventData.position);

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        invenUI.ExitDrag();
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"PointerDrag 이름{eventData.pointerDrag.name}");
        if (eventData.pointerDrag.TryGetComponent(out ItemSlot ChoisenSlot))
        {
            if (ChoisenSlot == this) return;

            Debug.Log($"드롭 {slotIdx}, {ChoisenSlot.slotIdx}");
            Managers.Game.Inventory.SwapItem(invenUI.curFilterType, invenUI.CurDragSlot.slotIdx, slotIdx);
        }
        else if (eventData.pointerDrag.TryGetComponent(out ConsumptionQuickSlot quickSlot))
        {
            var item = quickSlot.curItem;
            if (item == null) return;

            //TODO: 장비, 소비 나눌것

            Managers.Game.Inventory.FindItem(item, out int idx);
            Managers.Game.Inventory.SwapItem(invenUI.curFilterType, idx, slotIdx);
            Managers.Game.Inventory.RemoveQuickSlot(quickSlot.slotType);
        }
        else if (eventData.pointerDrag.TryGetComponent(out WeaponSlot slot))
        {

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Highlight(false);
        Managers.UI.CloseItemInfoUI();


    }
}
