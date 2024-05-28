using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreateCharacterUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform parent;
    [SerializeField] private SelectedCostumeButtonUI prefab;
    [SerializeField] private Button checkButton;

    private CreatureCustomizer _customizer;
    
    private bool _isPointerOverUI;
    public bool IsPointerOverUI => _isPointerOverUI;

    private void Awake()
    {
        Managers.UI.CreateCharacterUI = this;
    }

    private void Start()
    {
        _customizer = Managers.Game.Player.GetComponent<CreatureCustomizer>();
        checkButton.onClick.AddListener(Hide);
        
        foreach (var costume in _customizer.CostumeList)
        {
            var button = Instantiate(prefab, parent, false);
            button.Costume = costume;
            button.Init();
        }
    }
    
    public void Hide()
    {
        _isPointerOverUI = false;
        gameObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;
    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;
}