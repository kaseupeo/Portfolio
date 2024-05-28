using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image blindImage;
    [SerializeField] private Image usingBorderImage;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI remainInputCountText;
    [SerializeField] private TextMeshProUGUI keycodeText;

    private Skill _skill;
    private InputAction _input;

    public Skill Skill
    {
        get => _skill;
        set
        {
            if (_skill)
                _skill.OnStateChanged -= OnSkillStateChanged;

            _skill = value;

            if (_skill != null)
            {
                _skill.OnStateChanged += OnSkillStateChanged;

                iconImage.gameObject.SetActive(true);
                iconImage.sprite = _skill.Icon;
            }
            else
            {
                SetSkillUIActive(false);
            }
        }
    }

    private void Awake()
    {
        SetSkillUIActive(false);
    }

    private void Update()
    {
        if (!_skill)
            return;

        UpdateBlindImage();
        UpdateInput();
    }

    private void OnDestroy()
    {
        if (_skill)
            _skill.OnStateChanged -= OnSkillStateChanged;
    }

    public void Init(InputAction useInput)
    {
        _input = useInput;
        keycodeText.text = useInput.controls[0].displayName;
    }
    
    private void UpdateBlindImage()
    {
        if (_skill.IsInState<ReadyState>())
        {
            blindImage.gameObject.SetActive(!_skill.IsUseable);
        }
    }

    private void UpdateInput()
    {
        if (_skill.IsUseable && _input.IsPressed())
        {
            _skill.Owner.SkillSystem.CancelTargetSearching();
            _skill.Use();
        }
    }

    private void SetSkillUIActive(bool isOn)
    {
        cooldownText.gameObject.SetActive(isOn);
        blindImage.gameObject.SetActive(isOn);
        iconImage.gameObject.SetActive(isOn);
        remainInputCountText.gameObject.SetActive(isOn);
        usingBorderImage.gameObject.SetActive(isOn);
    }

    private void OnSkillStateChanged(Skill skill, State<Skill> currentState, State<Skill> prevState, int layer)
    {
        var stateType = currentState.GetType();

        if (layer == 0)
        {
            usingBorderImage.gameObject.SetActive(stateType != typeof(ReadyState));
        }

        if (stateType == typeof(CooldownState))
            StartCoroutine(ShowCooldown());
        else if (stateType == typeof(InActionState))
            StartCoroutine(ShowActionInfo());
    }

    private IEnumerator ShowActionInfo()
    {
        if (_skill.ApplyCycle > 0f)
            blindImage.gameObject.SetActive(true);

        if (_skill.ExecutionType == SkillExecutionType.Input)
        {
            remainInputCountText.gameObject.SetActive(true);
            _skill.OnCurrentApplyCountChanged += OnSkillCurrentApplyCountChanged;
            OnSkillCurrentApplyCountChanged(_skill, _skill.CurrentApplyCount, 0);
        }

        while (_skill.IsInState<InActionState>())
        {
            if (blindImage.gameObject.activeSelf)
                blindImage.fillAmount = 1f - (_skill.CurrentApplyCycle / _skill.ApplyCycle);

            if (_skill.Duration > 0f)
                usingBorderImage.fillAmount = 1f - (_skill.CurrentDuration / _skill.Duration);

            yield return null;
        }

        if (!_skill.IsInState<CooldownState>())
            blindImage.gameObject.SetActive(false);

        _skill.OnCurrentApplyCountChanged -= OnSkillCurrentApplyCountChanged;

        remainInputCountText.gameObject.SetActive(false);
        usingBorderImage.gameObject.SetActive(false);
        usingBorderImage.fillAmount = 1f;
    }

    private IEnumerator ShowCooldown()
    {
        blindImage.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);

        while (_skill.IsInState<CooldownState>())
        {
            cooldownText.text = _skill.CurrentCooldown.ToString("F1");
            blindImage.fillAmount = _skill.CurrentCooldown / _skill.Cooldown;
            yield return null;
        }

        blindImage.gameObject.SetActive(false);
        blindImage.fillAmount = 1f;

        cooldownText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_skill)
            Managers.UI.SkillTooltipUI.Show(_skill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_skill)
            Managers.UI.SkillTooltipUI.Hide();
    }

    private void OnSkillCurrentApplyCountChanged(Skill skill, int currentApplyCount, int prevApplyCount)
        => remainInputCountText.text = (skill.ApplyCount - currentApplyCount).ToString();

    public void OnDrop(PointerEventData eventData)
    {
        if (Managers.UI.DragHandler == null || Managers.UI.DragHandler.Skill == null)
            return;

        if (Managers.UI.DragHandler.GetComponentInParent<SkillSlotUI>())
            (Skill, Managers.UI.DragHandler.Skill) = (Managers.UI.DragHandler.Skill, Skill);
        else
            Skill = Managers.UI.DragHandler.Skill;

        Managers.UI.DragHandler.IsSucceedDrop = true;
    }
}