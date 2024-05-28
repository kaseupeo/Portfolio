using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    // Skill의 첫 적용에 ApplyCycle 시간만큼 Delay를 줄 것인가?
    // EX. ApplyCycle = 0.5, 0.5초 뒤부터 Skill 적용 시작 
    [SerializeField] private bool isDelayFirstApplyByCycle;
    // SkillObject가 Destroy되는 시간에 ApplyCycle만큼 지연을 줄 것인가?
    // EX. ApplyCycle = 0.5, 원래라면 3초에 Skill을 다 적용하고 Destroy 해야하지만 3.5초에 Destroy함
    [SerializeField] private bool isDelayDestroyByCycle;

    private float _currentDuration;
    private float _currentApplyCycle;
    private int _currentApplyCount;

    private TargetSearcher _targetSearcher;

    // Skill의 소유주
    public Creature Owner { get; private set; }
    // 이 SkillObject를 Spawn한 Skill
    public Skill Spawner { get; private set; }
    // SkillObject가 Skill을 적용할 Target을 찾기위한 TargetSearcher
    public TargetSearcher TargetSearcher => _targetSearcher;
    // SkillObject의 Transform Scale
    public Vector3 ObjectScale { get; private set; }
    public float Duration { get; private set; }
    public int ApplyCount { get; private set; }
    public float ApplyCycle { get; private set; }
    // SkillObject가 Destroy되는 시간.
    public float DestroyTime { get; private set; }

    private bool IsApplicable
        => (ApplyCount == 0 || _currentApplyCount < ApplyCount) && _currentApplyCycle >= ApplyCycle;

    public void Init(Skill spawner, TargetSearcher targetSearcher, float duration, int applyCount, Vector3 objectScale)
    {
        Spawner = spawner.Clone() as Skill;
        Owner = spawner.Owner;
        _targetSearcher = new TargetSearcher(targetSearcher);
        ApplyCount = applyCount;
        Duration = duration;
        ObjectScale = objectScale;
        ApplyCycle = CalculateApplyCycle(duration, applyCount);
        DestroyTime = Duration + (isDelayDestroyByCycle ? ApplyCycle : 0f);

        // SkillObject에 여러 설정을 적용하기위해 만들어진 SkillObjectComponent Script를 가져와서 Callback 함수를 호출해줌
        foreach (var component in GetComponents<ISkillObjectComponent>())
            component.OnSetupSkillObject(this);

        if (!isDelayFirstApplyByCycle)
            Apply();
    }

    private void OnDestroy()
    {
        Destroy(Spawner);
    }

    private void Update()
    {
        _currentDuration += Time.deltaTime;
        _currentApplyCycle += Time.deltaTime;

        if (IsApplicable)
            Apply();

        if (_currentDuration >= DestroyTime)
            Destroy(gameObject);
    }

    public float CalculateApplyCycle(float duration, int applyCount)
    {
        if (applyCount == 1)
            return 0f;
        
        return isDelayFirstApplyByCycle ? (duration / applyCount) : (duration / (applyCount - 1));
    }

    private void Apply()
    {
        _targetSearcher.SelectImmediate(Owner, gameObject, transform.position);
        var result = _targetSearcher.SearchTargets(Owner, gameObject);

        foreach (var target in result.TargetList)
            target.GetComponent<SkillSystem>().Apply(Spawner);

        _currentApplyCount++;
        _currentApplyCycle %= ApplyCycle;
    }
}