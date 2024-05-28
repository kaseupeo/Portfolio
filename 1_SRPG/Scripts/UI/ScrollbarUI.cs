using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollbarUI : MonoBehaviour
{
    [SerializeField] private List<string> textList;
    [SerializeField] private TextMeshProUGUI text;
    private int _step;
    
    private Scrollbar _scrollbar;
    private float _value;
    private int _index;

    public int Index => _index;

    private void Start()
    {
        _scrollbar = GetComponent<Scrollbar>();
        _step = textList.Count;
        _scrollbar.numberOfSteps = _step;
        
        if (_step != 0)
        {
            _value = (float)1 / (_step - 1);
            _scrollbar.size = _value / 2;
        }
        
        _scrollbar.onValueChanged.AddListener(FindIndex);
    }

    private void Update()
    {
        text.text = textList[_index];
    }

    public void Prev()
    {
        _scrollbar.value -= _value;
        if (_scrollbar.value <= 0)
        {
            _scrollbar.value = 0;
        }
    }

    public void Next()
    {
        _scrollbar.value += _value;
        if (_scrollbar.value >= 1)
        {
            _scrollbar.value = 1;
        }
    }

    private void FindIndex(float value)
    {
        _index = Mathf.RoundToInt(value * (_step - 1));
    }

    public float FindValue(int index)
    {
        return (float)index / (_step - 1);
    }
}
