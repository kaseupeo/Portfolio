using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stat : BaseObject
{
    public delegate void ValueChangedHandler(Stat stat, float currentValue, float prevValue);
    public event ValueChangedHandler OnValueChanged;
    public event ValueChangedHandler OnMaxValueChanged;
    public event ValueChangedHandler OnValueMax;
    public event ValueChangedHandler OnValueMin;
    
    [SerializeField] private bool isPercentType;
    [SerializeField] private float maxValue;
    [SerializeField] private float minValue;
    [SerializeField] private float defaultValue;

    private Dictionary<object, Dictionary<object, float>> _bonusValuesByKey = new();
    
    public bool IsPercentType => isPercentType;
    public float MaxValue
    {
        get => maxValue;
        set
        {
            float prevMaxValue = maxValue;

            maxValue = value;
            TryInvokeMaxValueChangedEvent(maxValue, prevMaxValue);
        }
    }

    public float MinValue { get => minValue; set => minValue = value; }
    public float DefaultValue
    {
        get => defaultValue;
        set
        {
            float prevValue = Value;
            
            defaultValue = Mathf.Clamp(value, MinValue, MaxValue);
            TryInvokeValueChangedEvent(Value, prevValue);
        }
    }

    public float BonusValue { get; private set; }
    public float Value => Mathf.Clamp(DefaultValue + BonusValue, MinValue, MaxValue);
    public bool IsMax => Mathf.Approximately(Value, maxValue);
    public bool IsMin => Mathf.Approximately(Value, minValue);

    private void TryInvokeValueChangedEvent(float currentValue, float prevValue)
    {
        if (Mathf.Approximately(currentValue, prevValue)) 
            return;
        
        OnValueChanged?.Invoke(this, currentValue, prevValue);

        if (Mathf.Approximately(currentValue, MaxValue))
            OnValueMax?.Invoke(this, MaxValue, prevValue);
        else if (Mathf.Approximately(currentValue, MinValue))
            OnValueMin?.Invoke(this, MinValue, prevValue);
    }

    private void TryInvokeMaxValueChangedEvent(float currentMaxValue, float prevMaxValue)
    {
        if (Mathf.Approximately(currentMaxValue, prevMaxValue))
            return;
        
        OnMaxValueChanged?.Invoke(this, currentMaxValue, prevMaxValue);
        
        if (IsMax) 
            OnValueChanged?.Invoke(this, MaxValue, Value);
        else if (IsMax)
            OnValueChanged?.Invoke(this, MinValue, Value);
    }
    
    public void SetBonusValue(object key, object subKey, float value)
    {
        if (!_bonusValuesByKey.ContainsKey(key))
            _bonusValuesByKey[key] = new Dictionary<object, float>();
        else
            BonusValue -= _bonusValuesByKey[key][subKey];

        float prevValue = Value;

        _bonusValuesByKey[key][subKey] = value;
        BonusValue += value;

        TryInvokeValueChangedEvent(Value, prevValue);
    }

    public void SetBonusValue(object key, float value) 
        => SetBonusValue(key, string.Empty, value);

    public float GetBonusValue(object key) 
        => _bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubKey)
        ? bonusValuesBySubKey.Sum(x => x.Value)
        : 0f;

    public float GetBonusValue(object key, object subKey)
    {
        if (_bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubKey))
        {
            if (bonusValuesBySubKey.TryGetValue(subKey, out var value))
                return value;
        }

        return 0f;
    }

    public bool RemoveBonusValue(object key)
    {
        if (_bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubKey))
        {
            float prevValue = Value;

            BonusValue -= bonusValuesBySubKey.Values.Sum();
            _bonusValuesByKey.Remove(key);

            TryInvokeValueChangedEvent(Value, prevValue);

            return true;
        }

        return false;
    }

    public bool RemoveBonusValue(object key, object subKey)
    {
        if (_bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubKey))
        {
            if (bonusValuesBySubKey.Remove(subKey, out var value))
            {
                float prevValue = Value;

                BonusValue -= value;

                TryInvokeValueChangedEvent(Value, prevValue);

                return true;
            }
        }

        return false;
    }

    public bool ContainsBonusValue(object key)
        => _bonusValuesByKey.ContainsKey(key);

    public bool ContainsBonusValue(object key, object subKey)
        => _bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubKey) 
           && bonusValuesBySubKey.ContainsKey(subKey);
}