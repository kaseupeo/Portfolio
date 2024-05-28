using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class SkillTooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI acquisitionText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private StringBuilder _stringBuilder = new();

    private void Awake()
    {
        Managers.UI.SkillTooltipUI = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Show(Skill skill)
    {
        acquisitionText.gameObject.SetActive(false);

        displayNameText.text = skill.DisplayName;
        var skillTypeName = skill.Type == SkillType.Active ? "액티브" : "패시브";
        skillTypeText.text = $"[{skillTypeName}]";
        costText.text = BuildCostText(skill);
        cooldownText.text = $"재사용 대기 시간: {skill.Cooldown:0.##}초";
        descriptionText.text = skill.Description;

        transform.position = Input.mousePosition;

        float xPivot = transform.localPosition.x > 0f ? 1f : 0f;
        float yPivot = transform.localPosition.y > 0f ? 1f : 0f;
        GetComponent<RectTransform>().pivot = new(xPivot, yPivot);

        gameObject.SetActive(true);
    }

    public void Show(SkillTreeSlotView slotView)
    {
        acquisitionText.gameObject.SetActive(false);
    
        if (slotView.RequesterOwnedSkill)
        {
            var ownedSkill = slotView.RequesterOwnedSkill;
    
            Show(ownedSkill);
    
            if (!ownedSkill.IsMaxLevel)
                TryShowConditionText(ownedSkill.Owner, "레벨 업 조건", ownedSkill.LevelUpConditions, ownedSkill.LevelUpCosts);
        }
        else
        {
            var creature = slotView.Requester;
            var slotSkill = slotView.SlotSkill;
            var temporarySkill = slotSkill.Clone() as Skill;
            temporarySkill.Init(creature);
    
            Show(temporarySkill);
            Destroy(temporarySkill);
    
            TryShowConditionText(creature, "습득 조건", slotSkill.AcquisitionConditions, slotSkill.AcquisitionCosts);
        }
    }

    public void Hide() => gameObject.SetActive(false);

    private void TryShowConditionText(Creature creature, string prefixText,
        IReadOnlyList<CreatureCondition> conditions, IReadOnlyList<Cost> costs)
    {
        if (conditions.Count == 0 && costs.Count == 0)
            return;

        acquisitionText.gameObject.SetActive(true);
        acquisitionText.text = BuildConditionText(creature, prefixText, conditions, costs);
    }

    private string BuildCostText(Skill skill)
    {
        _stringBuilder.Append("비용: ");

        if (skill.IsToggleType)
            _stringBuilder.Append("초당 ");

        int costLength = skill.Costs.Count;
        for (int i = 0; i < costLength; i++)
        {
            var cost = skill.Costs[i];
            _stringBuilder.Append(cost.Description);
            _stringBuilder.Append(' ');
            _stringBuilder.Append(cost.GetValue(skill.Owner));

            if (i != costLength - 1)
                _stringBuilder.Append(", ");
        }

        var result = _stringBuilder.ToString();
        _stringBuilder.Clear();

        return result;
    }

    private string BuildConditionText(Creature creature, string prefixText,
    IReadOnlyList<CreatureCondition> conditions, IReadOnlyList<Cost> costs)
    {
        _stringBuilder.Append(prefixText);
        _stringBuilder.Append(": ");

        for (int i = 0; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            _stringBuilder.Append("<color=");
            _stringBuilder.Append(condition.IsPass(creature) ? "white>" : "red>");
            _stringBuilder.Append(condition.Description);
            _stringBuilder.Append("</color>");

            if (i != conditions.Count - 1)
                _stringBuilder.Append(", ");
        }

        if (conditions.Count > 0)
            _stringBuilder.Append(", ");

        for (int i = 0; i < costs.Count; i++)
        {
            var cost = costs[i];

            _stringBuilder.Append("<color=");
            _stringBuilder.Append(cost.HasEnoughCost(creature) ? "white>" : "red>");
            _stringBuilder.Append(costs[i].Description);
            _stringBuilder.Append(' ');
            _stringBuilder.Append(costs[i].GetValue(creature));
            _stringBuilder.Append("</color>");

            if (i != costs.Count - 1)
                _stringBuilder.Append(", ");
        }

        string result = _stringBuilder.ToString();
        _stringBuilder.Clear();

        return result;
    }
}
