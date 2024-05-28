using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDiffPanelUI : MonoBehaviour
{
    [SerializeField] private Toggle togglePrefab;

    private ToggleGroup _toggleGroup;
    private List<MapDiffToggleUI> _toggles;
    private List<GameObject> _map;

    private void Start()
    {
        _toggleGroup = GetComponent<ToggleGroup>();
        _toggles = new List<MapDiffToggleUI>();
        _map = Managers.Map.LoadMaps;
        
        foreach (GameObject map in _map)
        {
            var toggle = Instantiate(togglePrefab, _toggleGroup.transform).gameObject.AddComponent<MapDiffToggleUI>();
            toggle.SetMap(map);
            _toggles.Add(toggle);
            toggle.GetComponent<Toggle>().group = _toggleGroup;
        }
    }
}
