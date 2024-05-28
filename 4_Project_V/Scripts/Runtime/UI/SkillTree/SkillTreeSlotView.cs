using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillTreeSlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public delegate void SkillAcquiredHandler(SkillTreeSlotView slotView, Skill skill);

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject normalBorder;
    [SerializeField] private GameObject acquiredBorder;
    [SerializeField] private GameObject blind;

    private Creature _requester;
    private Skill _requesterOwnedSkill;

    public Creature Requester => _requester;
    // Requester의 SkillSystem에서 찾아온 SlotSkill => Entity가 소유한 Skill
    public Skill RequesterOwnedSkill => _requesterOwnedSkill;
    public Skill SlotSkill => SlotNode.Skill;
    public SkillTreeSlotNode SlotNode { get; private set; }

    public event SkillAcquiredHandler OnSkillAcquired;

    private void Update()
    {
        if (_requesterOwnedSkill)
            return;

        bool isAcquirable = SlotNode.IsSkillAcquirable(_requester);
        blind.SetActive(!isAcquirable);
    }
    
    private void OnDestroy()
    {
        if (!_requester)
            return;

        _requester.SkillSystem.OnSkillRegistered -= OnSkillRegistered;
    }
    
    public void SetViewTarget(Creature requester, SkillTreeSlotNode slotNode)
    {
        if (requester)
            requester.SkillSystem.OnSkillRegistered -= OnSkillRegistered;

        _requester = requester;
        SlotNode = slotNode;

        var skill = slotNode.Skill;

        _requesterOwnedSkill = requester.SkillSystem.Find(skill);
        if (!_requesterOwnedSkill)
            requester.SkillSystem.OnSkillRegistered += OnSkillRegistered;

        iconImage.sprite = skill.Icon;

        UpdateAcquisitionUI();
        UpdateLevelText();
    }

    private void UpdateAcquisitionUI()
    {
        normalBorder.SetActive(!_requesterOwnedSkill);
        acquiredBorder.SetActive(_requesterOwnedSkill);
        blind.SetActive(!_requesterOwnedSkill && !SlotNode.IsSkillAcquirable(_requester));
    }

    private void UpdateLevelText()
    {
        int level = _requesterOwnedSkill ? _requesterOwnedSkill.Level : 0;
        levelText.text = $"{level} / {SlotNode.Skill.MaxLevel}";
        levelText.color = (_requesterOwnedSkill && _requesterOwnedSkill.IsMaxLevel) ? Color.yellow : Color.white;
    }

    private void ShowTooltip()
    {
        Managers.UI.SkillTooltipUI.Show(this);
    }

    private void HideTooltip()
    {
        Managers.UI.SkillTooltipUI.Hide();
    }

    private void OnSkillRegistered(SkillSystem skillSystem, Skill skill)
    {
        if (skill.ID != SlotNode.Skill.ID)
            return;

        _requesterOwnedSkill = skill;

        UpdateAcquisitionUI();
        UpdateLevelText();

        skillSystem.OnSkillRegistered -= OnSkillRegistered;

        OnSkillAcquired?.Invoke(this, _requesterOwnedSkill);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        => ShowTooltip();

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        => HideTooltip();

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        // Entity가 skill을 소유했고, Level Up 조건을 달성 했다면 Level Up을 시킴
        if (_requesterOwnedSkill && _requesterOwnedSkill.IsCanLevelUp)
        {
            _requesterOwnedSkill.LevelUp();
            UpdateLevelText();
            ShowTooltip();
        }
        // Entity가 Skill을 소유하지 않았고, Skill을 습득할 수 있는 상태라면 습득함
        else if (!_requesterOwnedSkill && SlotNode.IsSkillAcquirable(_requester))
        {
            SlotNode.AcquireSkill(_requester);
            ShowTooltip();
        }
    }
}
