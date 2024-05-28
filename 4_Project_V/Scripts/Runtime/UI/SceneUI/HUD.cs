using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HUD : SceneUI, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isPointerOverUI;
    public bool IsPointerOverUI => _isPointerOverUI;
    
    protected override void Awake()
    {
        Managers.UI.HUD = this;
        base.Awake();
        //buttons["SettingButton"].onClick.AddListener(() => { OpenPausePopupUI(); });
        //buttons["VolumeButton"].onClick.AddListener(() => { Debug.Log("Volume"); });
        //buttons["InfoButton"].onClick.AddListener(() => { OpenInfoWindowUI(); });
    }

    public void OpenPausePopupUI()
    {
        //Managers.UI.ShowPopupUI<PopupUI>("Prefabs/UI/SettingPopupUI");
    }
    public void OpenInfoWindowUI()
    {
        //Managers.UI.ShowWindowUI<WindowUI>("Prefabs/UI/InfoWindowUI");
    }

    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;
    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;
}