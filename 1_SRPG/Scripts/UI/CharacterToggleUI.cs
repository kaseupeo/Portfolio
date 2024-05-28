using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterToggleUI : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _text;
    private Toggle _toggle;

    private PlayerCharacter _playerCharacter;
    
    public void SetPlayerCharacter(PlayerCharacter playerCharacter)
    {
        _toggle = GetComponent<Toggle>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.text = $"{playerCharacter.Name}";
        _playerCharacter = playerCharacter;
    }

    private void Update()
    {
        _text.color = _toggle.isOn ? Color.red : Color.blue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) 
            Managers.Game.SelectedCharacter = _toggle.isOn ? _playerCharacter : null;
        
    }
}