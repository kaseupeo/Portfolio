using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillBarUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private int slotCount = 6;

    private SkillSystem _skillSystem;
    private Dictionary<int, SkillSlotUI> _slotDic = new();
    private int _emptySlotIndex;

    private void Start()
    {
        _skillSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillSystem>();
        _skillSystem.OnSkillRegistered += OnSkillRegistered;

        // TODO : 스킬 UI 만들고 스킬 슬롯에 배치하는거 만들고 변경 필요
        var quickSlotSkillList = _skillSystem.QuickSlotSkillList;

        for (int i = 0; i < slotCount; i++)
        {
            var slot = Instantiate(slotPrefab, transform).GetComponent<SkillSlotUI>();
            slot.Init(Managers.Input.SkillAction[i + 1]);
            _slotDic.Add(i, slot);

            if (i < quickSlotSkillList.Count)
                TryAddToEmptySlot(quickSlotSkillList[i]);
        }
    }
    
    private void OnDestroy() => _skillSystem.OnSkillRegistered -= OnSkillRegistered;

    private void TryAddToEmptySlot(Skill skill)
    {
        if (skill.IsPassive)
            return;

        for (int i = 0; i < slotCount; i++)
        {
            if (_slotDic[i].Skill == null)
            {
                _slotDic[i].Skill = skill;
                break;
            }
        }
    }

    private void OnSkillRegistered(SkillSystem skillSystem, Skill skill)
        => TryAddToEmptySlot(skill);
}