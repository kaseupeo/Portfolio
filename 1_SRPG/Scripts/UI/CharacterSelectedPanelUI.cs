using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectedPanelUI : MonoBehaviour
{
    [SerializeField] private Toggle togglePrefab;

    private ToggleGroup _toggleGroup;

    private List<PlayerCharacter> _playerCharacters;
    private List<CharacterToggleUI> _toggles;


    private void OnEnable()
    {
        _toggleGroup = GetComponentInChildren<ToggleGroup>();
        _playerCharacters = new List<PlayerCharacter>();
        _toggles = new List<CharacterToggleUI>();

        _playerCharacters = Managers.Game.LoadPlayerCharacters;
        
        for (int i = 0; i < _playerCharacters.Count; i++)
        {
            var toggle = Instantiate(togglePrefab, _toggleGroup.transform).gameObject.AddComponent<CharacterToggleUI>();
            toggle.SetPlayerCharacter(_playerCharacters[i]);
            _toggles.Add(toggle);
            toggle.GetComponent<Toggle>().group = _toggleGroup;
            toggle.GetComponent<Toggle>().isOn = false;
        }
    }

    private void OnDisable()
    {
        foreach (CharacterToggleUI toggle in _toggles)
        {
            Destroy(toggle.gameObject);
        }
    }
}
