using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIManager
{
    public Action<bool> IsOpenUIAction;
    public delegate void ChangedCostumeHandler(Category category);

    public ChangedCostumeHandler OnPrevCostume;
    public ChangedCostumeHandler OnNextCostume;
    
    public HUD HUD { get; set; }
    public FloatingTextView FloatingTextView { get; set; }
    public SkillTooltipUI SkillTooltipUI { get; set; }
    public SkillTreeView SkillTreeView { get; set; }
    public CharacterStatUI CharacterStatUI { get; set; }
    public InventoryUI InventoryUI { get; set; }
    public ItemQuickSlotUI ItemQuickSlot { get; set; }
    public QuestUI QuestUI { get; set; }
    public DragHandler DragHandler { get; set; }
    public LoadingUI LoadingUI { get; set; }
    public CreateCharacterUI CreateCharacterUI { get; set; }
    public ShopPopupUI ShopPopupUI { get; set; }
    public BossMonsterHUD BossMonsterHUD { get; set; }
    public ItemInfoPopupUI ItemInfoPopupUI { get; set; }
    public MainMenuUI MainMenuUI { get; set; }

    public bool IsPointerOverUI
        => HUD.IsPointerOverUI || CharacterStatUI.IsPointerOverUI || SkillTreeView.IsPointerOverUI || QuestUI.IsPointerOverUI ||
           InventoryUI.IsPointerOverUI || CreateCharacterUI.IsPointerOverUI || ShopPopupUI.IsPointerOverUI || MainMenuUI.IsPointerOverUI ||
           (DragHandler != null && DragHandler.IsPointerOverUI);
    
    
    private Canvas popupCanvas;
    private Canvas windowCanvas;
    private Canvas ingameCanvas;

    private Stack<PopupUI> popupStack = new();

    public void Init()
    {
        Managers.Resource.Instantiate("UI/EventSystem", GameObject.Find("Managers").transform).GetComponent<EventSystem>();

        popupCanvas = Managers.Resource.Instantiate<Canvas>($"UI/Canvas");
        popupCanvas.gameObject.name = "PopupCanvas";
        popupCanvas.sortingOrder = 100;
        popupCanvas.transform.SetParent(Managers.Instance.transform);

        ItemInfoPopupUI =  Managers.Resource.Instantiate("UI/Popup/ItemInfoPanel", popupCanvas.transform).GetComponent<ItemInfoPopupUI>();
        ItemInfoPopupUI.gameObject.SetActive(false);


      

        // windowCanvas = Managers.Resource.Instantiate<Canvas>($"UI/Canvas");
        // windowCanvas.gameObject.name = "WindowCanvas";
        // windowCanvas.sortingOrder = 10;
        // windowCanvas.transform.SetParent(Managers.Instance.transform);
        //
        // ingameCanvas = Managers.Resource.Instantiate<Canvas>($"UI/Canvas");
        // ingameCanvas.gameObject.name = "IngameCanvas";
        // ingameCanvas.sortingOrder = 0;
        // ingameCanvas.transform.SetParent(Managers.Instance.transform);

    }

    public T ShowPopupUI<T>(T popupUI) where T : PopupUI
    {
        if (popupStack.Count > 0)
        {
            PopupUI prevUI = popupStack.Peek();
            prevUI.gameObject.SetActive(false);
        }

        T ui = Managers.Pool.GetUI(popupUI);
        ui.transform.SetParent(popupCanvas.transform, false);

        popupStack.Push(ui);
        if (popupStack.Count == 1)
        {
           
        }

        return ui;
    }

    public T ShowPopupUI<T>(string path) where T : PopupUI
    {
        T ui = Managers.Resource.Load<T>(path);
        return ShowPopupUI(ui);

    }

    public void ClosePopupUI()
    {

        PopupUI ui = popupStack.Pop();
        Managers.Pool.ReleaseUI(ui);

        if (popupStack.Count > 0)
        {
            PopupUI curPop = popupStack.Peek();
            curPop.gameObject.SetActive(true);
        }

        if (popupStack.Count == 0)
        {
           
        }
    }

    public T ShowWindowUI<T>(T windowUI) where T : WindowUI
    {

        T ui = Managers.Pool.GetUI(windowUI);
        ui.transform.SetParent(windowCanvas.transform, false);
        return ui;
    }

    public T ShowWindowUI<T>(string path) where T : WindowUI
    {
        T ui = Managers.Resource.Load<T>(path);
        return ShowWindowUI(ui);

    }
    public void SelecteWindow(WindowUI windowUI)
    {
        windowUI.transform.SetAsLastSibling();
    }
    public void CloseWindowUI<T>(T windowUI) where T : WindowUI
    {
        Managers.Pool.ReleaseUI(windowUI.gameObject);
    }

    public T ShowIngameUI<T>(T ingameUI) where T : IngameUI
    {
        T ui = Managers.Pool.GetUI(ingameUI);
        ui.transform.SetParent(ingameCanvas.transform, false);
        return ui;
    }
    public T ShowIngameUI<T>(string path) where T : IngameUI
    {
        T ui = Managers.Resource.Load<T>(path);
        return ShowIngameUI(ui);

    }

    public void CloseIngameUI<T>(T ingameUI) where T : IngameUI
    {
        //Managers.Pool.ReleaseUI(ingameUI.gameObject);

    }

    public void OpenItemInfoUI(Item item, Vector2 position)
    {
        ItemInfoPopupUI.gameObject.SetActive(true);
        ItemInfoPopupUI.SetItem(item, position);
    }
    public void CloseItemInfoUI()
    {
        ItemInfoPopupUI.gameObject.SetActive(false);
    }


}