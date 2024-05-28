using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Color chargeColor = Color.yellow;

    private SkillSystem _skillSystem;

    private void Start()
    {
        _skillSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillSystem>();
        _skillSystem.onSkillStateChanged += OnSkillStateChanged;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _skillSystem.onSkillStateChanged -= OnSkillStateChanged;
    }

    private void InfoUpdate(float currentTime, float maxTime)
    {
        fillImage.fillAmount = currentTime / maxTime;
        timeText.text = $"{currentTime:F1} : {maxTime:F1}";
    }

    private void OnSkillStateChanged(SkillSystem skillSystem, Skill skill,
        State<Skill> currentSkill, State<Skill> prevState, int layer)
    {
        if (skill.IsInState<CastingState>())
        {
            gameObject.SetActive(true);
            StartCoroutine(CastingProgressUpdate(skill));
            
        }
        else if (skill.IsInState<ChargingState>())
        {
            gameObject.SetActive(true);
            StartCoroutine(ChargingProgressUpdate(skill));
        }
    }

    private IEnumerator CastingProgressUpdate(Skill skill)
    {
        while (skill.IsInState<CastingState>())
        {
            InfoUpdate(skill.CurrentCastTime, skill.CastTime);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private IEnumerator ChargingProgressUpdate(Skill skill)
    {
        var defaultColor = fillImage.color;
        while (skill.IsInState<ChargingState>())
        {
            InfoUpdate(skill.CurrentChargeDuration, skill.ChargeDuration);
            if (skill.IsMinChargeCompleted)
                fillImage.color = chargeColor;

            yield return null;
        }

        fillImage.color = defaultColor;
        gameObject.SetActive(false);
    }
}
