using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Slot slotPrefab;
    [SerializeField] private GameObject toolTipPrefab;
    private List<Slot> _slots;
    private List<Item> _items;

    private GameObject _toolTip;
    
    public GameObject ToolTip => _toolTip;

    private void Update()
    {
        if (Managers.Game.SelectedCharacter == null && Managers.Game.SelectedCharacter.State != Define.State.Inventory)
            gameObject.SetActive(false);
    }

    private void Init()
    {
        _items = new List<Item>();
        _slots = new List<Slot>();
        
        for (int i = 0; i < 10; i++)
        {
            var go = Instantiate(slotPrefab, transform);
            _slots.Add(go);
        }

        _toolTip = Instantiate(toolTipPrefab, transform.parent);
        _toolTip.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);

        if (_slots == null || _slots.Count == 0) 
            Init();

        _items = Managers.Game.SelectedCharacter.Items;

        for (int i = 0; i < _items.Count; i++)
        {
            _slots[i].SetItem(_items[i]);
        }
    }

    public void Close()
    {
        if (_slots == null)
            return;
        
        foreach (var slot in _slots)
            slot.ResetSlot();

        gameObject.SetActive(false);
    }

    public void Acquire(Item item)
    {
        
    }
}