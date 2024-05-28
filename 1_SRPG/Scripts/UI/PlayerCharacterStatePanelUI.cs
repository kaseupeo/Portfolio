using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerCharacterStatePanelUI : MonoBehaviour
{
    [SerializeField] private Toggle togglePrefab;
    [SerializeField] private Inventory inventory;
    
    private ToggleGroup _toggleGroup;
    private List<StateToggleUI> _toggles;
    
    private void OnEnable()
    {
        _toggleGroup = GetComponent<ToggleGroup>();
        _toggles = new List<StateToggleUI>();
        
        // MOVE
        {
            var toggle = Instantiate(togglePrefab, _toggleGroup.transform).gameObject.AddComponent<StateToggleUI>();
            toggle.SetCharacterState(Define.State.Move);
            _toggles.Add(toggle);
            toggle.GetComponent<Toggle>().group = _toggleGroup;
            toggle.GetComponent<Toggle>().isOn = false;
        }
        // ATTACK
        {
            var toggle = Instantiate(togglePrefab, _toggleGroup.transform).gameObject.AddComponent<StateToggleUI>();
            toggle.SetCharacterState(Define.State.Attack);
            _toggles.Add(toggle);
            toggle.GetComponent<Toggle>().group = _toggleGroup;
            toggle.GetComponent<Toggle>().isOn = false;
        }
        // INVENTORY
        {
            var toggle = Instantiate(togglePrefab, _toggleGroup.transform).gameObject.AddComponent<StateToggleUI>();
            toggle.SetCharacterState(Define.State.Inventory);
            _toggles.Add(toggle);
            toggle.GetComponent<Toggle>().group = _toggleGroup;
            toggle.GetComponent<Toggle>().isOn = false;
        }
    }

    private void Update()
    {
        foreach (StateToggleUI toggle in _toggles)
        {
            if (Managers.Game.SelectedCharacter == null)
            {
                toggle.GetComponent<Toggle>().isOn = false;
                continue;
            }
            toggle.GetComponent<Toggle>().isOn = Managers.Game.SelectedCharacter.State == toggle.State;

            if (toggle.State == Define.State.Inventory && toggle.GetComponent<Toggle>().isOn)
                inventory.Open();
            else
                inventory.Close();
        }
    }

    private void OnDisable()
    {
        foreach (StateToggleUI toggle in _toggles)
        {
            Destroy(toggle.gameObject);
        }
    }
}
