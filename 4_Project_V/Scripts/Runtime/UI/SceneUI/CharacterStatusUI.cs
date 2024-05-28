using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusUI : MonoBehaviour
{
    [SerializeField] private Image hp;
    [SerializeField] private Image mp;
    
    private Stats _stats;
    
    private void Start()
    {
        _stats = GameObject.FindGameObjectWithTag("Player").GetComponent<Stats>();
        
        hp.fillAmount = 1;
        mp.fillAmount = 1;
        
        _stats.HPStat.OnValueChanged += OnChangedHPValue;
        _stats.MPStat.OnValueChanged += OnChangedMPValue;
    }

    private void OnChangedHPValue(Stat stat, float value, float prevValue) => hp.fillAmount = value / stat.MaxValue;
    private void OnChangedMPValue(Stat stat, float value, float prevValue) => mp.fillAmount = value / stat.MaxValue;
}