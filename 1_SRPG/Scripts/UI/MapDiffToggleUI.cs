using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapDiffToggleUI : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _text;
    private Toggle _toggle;
    private GameObject _gameObject;

    public void SetMap(GameObject go)
    {
        _toggle = GetComponent<Toggle>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.text = $"{go.name}";
        _gameObject = go;

        if (_toggle.isOn) 
            Managers.Game.Map = _gameObject;
    }

    private void Update()
    {
        _text.color = _toggle.isOn ? Color.red : Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Managers.Game.Map = _gameObject;
        }
    }
}