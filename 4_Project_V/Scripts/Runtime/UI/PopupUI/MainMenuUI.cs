using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuUI : PopupUI, IPointerEnterHandler, IPointerExitHandler
{
    private bool _isPointerOverUI;
    public bool IsPointerOverUI => _isPointerOverUI;
    string message = "정말 게임을 종료하시겠습니까?";
    protected override void Awake()
    {
        base.Awake();
        Managers.UI.MainMenuUI = this;
        ShowText();
        buttons["TitleButton"].onClick.AddListener(() => 
        {
            Managers.Scene.LoadScene(Define.SceneType.TitleScene);
        });
        buttons["ExitConfirmButton"].onClick.AddListener(() =>
        {
            Managers.Game.Quit();
        });
        buttons["ExitCancelButton"].onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        gameObject.SetActive(false);
    }
    public void Show(Creature creature)
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _isPointerOverUI = false;
        gameObject.SetActive(false);
    }
    public void ShowText()
    {
        texts["ExitText"].text = message;
    }
    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;
    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;

}
