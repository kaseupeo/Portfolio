using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StateToggleUI : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _text;
    private string _stateName;
    private Toggle _toggle;
    private Define.State _state;
    
    public Define.State State => _state;

    public void SetCharacterState(Define.State state)
    {
        switch (state)
        {
            case Define.State.Idle:
                _stateName = "대기";
                break;
            case Define.State.Move:
                _stateName = "이동";
                break;
            case Define.State.Attack:
                _stateName = "공격";
                break;
            case Define.State.Inventory:
                _stateName = "가방";
                break;
        }
        
        _toggle = GetComponent<Toggle>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.text = $"{_stateName}";
        _state = state;
    }

    private void Update()
    {
        _text.color = _toggle.isOn ? Color.red : Color.blue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Managers.Game.SelectedCharacter == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
            Managers.Game.SelectedCharacter.State = _toggle.isOn ? _state : Define.State.Idle;
    }
}
