using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSlotUI : MonoBehaviour
{
    public delegate void StatPointChangedHandler(Stat stat, int point);

    public event StatPointChangedHandler OnStatChanged;
    
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI point;
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;

    private bool _isShowMaxStat;

    public void Init(Stat stat, bool isShowMaxStat = false)
    {
        stat.OnValueChanged += OnValueChanged;
        stat.OnMaxValueChanged += OnValueChanged;

        _isShowMaxStat = isShowMaxStat;

        icon.sprite = stat.Icon;
        statName.text = stat.DisplayName;

        OnValueChanged(stat, stat.Value, 0);
        
        if (plusButton == null)
            return;
        
        plusButton.onClick.AddListener(() =>
        {
            OnStatChanged?.Invoke(stat, 1);
            OnValueChanged(stat, stat.Value, 0);
        });
        
        // minusButton.onClick.AddListener(() =>
        // {
        //     OnStatChanged?.Invoke(_stat, -1);
        //     _stat.DefaultValue--;
        //     point.message = $"{_stat.DefaultValue}";
        // });
    }

    private void OnValueChanged(Stat stat, float value, float prevValue)
    {
        point.text = $"{value}";
        
        if (stat.BonusValue != 0)
        {
            if (stat.IsMax)
                point.text += $"({Mathf.Clamp(stat.BonusValue, stat.MinValue, stat.MaxValue) - stat.DefaultValue})";
            else
                point.text += $"({stat.BonusValue})";
        }
        
        if (_isShowMaxStat) 
            point.text += $" / {stat.MaxValue}";
        
        if (plusButton != null)
            plusButton.interactable = !stat.IsMax;
    }
}