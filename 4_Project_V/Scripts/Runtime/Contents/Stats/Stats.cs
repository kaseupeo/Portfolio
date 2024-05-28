using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("기본 스탯")] 
    [SerializeField, Tooltip("물공")] private Stat strStat;
    [SerializeField, Tooltip("마공")] private Stat intStat;
    [SerializeField, Tooltip("체력")] private Stat conStat;
    [SerializeField, Tooltip("마나")] private Stat wisStat;
    [SerializeField, Tooltip("스피드")] private Stat dexStat;
    
    [Header("레벨 관련")]
    [SerializeField] private Stat levelStat;
    [SerializeField] private Stat expStat;
    [SerializeField] private Stat statPoint;
    [SerializeField] private Stat skillPoint;
    
    [Header("실 스탯")]
    [SerializeField] private Stat hpStat;
    [SerializeField] private Stat mpStat;
    [SerializeField] private Stat hpRecoveryStat;
    [SerializeField] private Stat mpRecoveryStat;
    [SerializeField] private Stat moveSpeedStat;
    [SerializeField] private Stat attackRangeStat;

    [Header("돈")] 
    [SerializeField] private Stat goldStat;
    
    [SerializeField] private StatOverride[] statOverrides;

    private Stat[] _stats;
    private List<Stat> _originalStatList = new();
    private List<Stat> _levelStatList = new();
    private List<Stat> _realStatList = new();
    
    public Creature Owner { get; private set; }
    
    public Stat STRStat { get; private set; }
    public Stat INTStat { get; private set; }
    public Stat CONStat { get; private set; }
    public Stat WISStat { get; private set; }
    public Stat DEXStat { get; private set; }
    
    public Stat LevelStat { get; private set; }
    public Stat ExpStat { get; private set; }
    public Stat StatPoint { get; private set; }
    public Stat SkillPoint { get; private set; }

    public Stat HPStat { get; private set; }
    public Stat MPStat { get; private set; }
    public Stat HPRecoveryStat { get; private set; }
    public Stat MPRecoveryStat { get; private set; }
    public Stat MoveSpeedStat { get; private set; }
    public Stat AttackRangeStat { get; private set; }

    public Stat GoldStat { get; private set; }

    public IReadOnlyList<Stat> OriginalStatList => _originalStatList;
    public IReadOnlyList<Stat> LevelStatList => _levelStatList;
    public IReadOnlyList<Stat> RealStatList => _realStatList;

    public void Init(Creature creature)
    {
        Owner = creature;

        _stats = statOverrides.Select(x => x.CreateStat()).ToArray();
        
        STRStat = strStat ? GetStat(strStat) : null;
        INTStat = intStat ? GetStat(intStat) : null;
        CONStat = conStat ? GetStat(conStat) : null;
        WISStat = wisStat ? GetStat(wisStat) : null;
        DEXStat = dexStat ? GetStat(dexStat) : null;
        
        LevelStat = levelStat ? GetStat(levelStat) : null;
        ExpStat = expStat ? GetStat(expStat) : null;
        StatPoint = statPoint ? GetStat(statPoint) : null;
        SkillPoint = skillPoint ? GetStat(skillPoint) : null;
        
        HPStat = hpStat ? GetStat(hpStat) : null;
        MPStat = mpStat ? GetStat(mpStat) : null;
        HPRecoveryStat = hpRecoveryStat ? GetStat(hpRecoveryStat) : null;
        MPRecoveryStat = mpRecoveryStat ? GetStat(mpRecoveryStat) : null;
        MoveSpeedStat = moveSpeedStat ? GetStat(moveSpeedStat) : null;
        AttackRangeStat = attackRangeStat ? GetStat(attackRangeStat) : null;

        GoldStat = goldStat ? GetStat(goldStat) : null;
        
        SettingStat();
    }

    private void SettingStat()
    {
        _originalStatList.Add(STRStat);
        _originalStatList.Add(INTStat);
        _originalStatList.Add(CONStat);
        _originalStatList.Add(WISStat);
        _originalStatList.Add(DEXStat);
        
        _levelStatList.Add(LevelStat);
        _levelStatList.Add(ExpStat);
        _levelStatList.Add(SkillPoint);
        _levelStatList.Add(StatPoint);
        
        _realStatList.Add(HPStat);
        _realStatList.Add(MPStat);
        _realStatList.Add(HPRecoveryStat);
        _realStatList.Add(MPRecoveryStat);
        // _realStatList.Add(AttackRangeStat);

        ChangedStat();
        HPStat.DefaultValue = HPStat.MaxValue;
        MPStat.DefaultValue = MPStat.MaxValue;
    }

    public void ChangedStat()
    {
        HPStat.MaxValue = CONStat.Value * 10;
        MPStat.MaxValue = WISStat.Value * 10;
        
        if (HPRecoveryStat) 
            HPRecoveryStat.DefaultValue = CONStat.Value / 10;
        if (MPRecoveryStat)
            MPRecoveryStat.DefaultValue = WISStat.Value / 10;
        if (MoveSpeedStat)
            MoveSpeedStat.DefaultValue = DEXStat.Value / 2;
    }
    
    public Stat GetStat(Stat stat)
    {
        Debug.Assert(stat != null, $"Stats::GetStat - stat은 null이 될 수 없습니다.");
        
        return _stats.FirstOrDefault(x => x.ID == stat.ID);
    }

    public bool TryGetStat(Stat stat, out Stat outStat)
    {
        Debug.Assert(stat != null, $"Stats::TryGetStat - stat은 null이 될 수 없습니다.");
        outStat = _stats.FirstOrDefault(x => x.ID == stat.ID);

        return outStat != null;
    }

    public float GetValue(Stat stat)
        => GetStat(stat).Value;

    public bool HasStat(Stat stat)
    {
        Debug.Assert(stat != null, $"Stats::HasStat - stat은 null이 될 수 없습니다.");
        return _stats.Any(x => x.ID == stat.ID);
    }

    public float GetDefaultValue(Stat stat)
        => GetStat(stat).DefaultValue;
    
    public void SetDefaultValue(Stat stat, float value)
        => GetStat(stat).DefaultValue = value;
    
    public void IncreaseDefaultValue(Stat stat, float value)
        => GetStat(stat).DefaultValue += value;

    public void IncreaseMaxValue(Stat stat, float value)
        => GetStat(stat).MaxValue += value;
    
    public float GetBonusValue(Stat stat)
        => GetStat(stat).BonusValue;

    public float GetBonusValue(Stat stat, object key)
        => GetStat(stat).GetBonusValue(key);

    public float GetBonusValue(Stat stat, object key, object subKey)
        => GetStat(stat).GetBonusValue(key, subKey);
    
    public void SetBonusValue(Stat stat, object key, float value)
        => GetStat(stat).SetBonusValue(key, value);

    public void SetBonusValue(Stat stat, object key, object subKey, float value)
        => GetStat(stat).SetBonusValue(key, subKey, value);

    public void RemoveBonusValue(Stat stat, object key)
        => GetStat(stat).RemoveBonusValue(key);

    public void RemoveBonusValue(Stat stat, object key, object subKey)
        => GetStat(stat).RemoveBonusValue(key, subKey);

    public bool ContainsBonusValue(Stat stat, object key)
        => GetStat(stat).ContainsBonusValue(key);

    public bool ContainsBonusValue(Stat stat, object key, object subKey)
        => GetStat(stat).ContainsBonusValue(key, subKey);
    
    private void OnDestroy()
    {
        foreach (var stat in _stats)
            Destroy(stat);
        _stats = null;
    }
    
    
#if UNITY_EDITOR
    [ContextMenu("load Stats")]
    private void LoadStats()
    {
        var stats = Resources.LoadAll<Stat>("Stat").OrderBy(x => x.ID);
        statOverrides = stats.Select(x => new StatOverride(x)).ToArray();
    }
    
    private void OnGUI()
    {
        if (!Owner.IsPlayer)
            return;
        
        // 좌측 상단에 넓은 Box를 그려줌
        GUI.Box(new Rect(2f, 2f, 300f, 500f), string.Empty);

        // 박스 윗 부분에 Player Stat Text를 뜨워줌
        GUI.Label(new Rect(4f, 2f, 100f, 30f), "Player Stat");

        var textRect = new Rect(4f, 22f, 200f, 30f);
        // Stat 증가를 위한 + Button의 기준 위치
        var plusButtonRect = new Rect(textRect.x + textRect.width, textRect.y, 20f, 20f);
        // Stat 감소를 위한 - Button의 기준 위치
        var minusButtonRect = plusButtonRect;
        minusButtonRect.x += 22f;

        foreach (var stat in _stats)
        {
            // % Type이면 곱하기 100을 해서 0~100으로 출력
            // 0.##;-0.## format은 소숫점 2번째짜리까지 출력하되
            // 양수면 그대로 출력, 음수면 -를 붙여서 출력하라는 것
            string defaultValueAsString = stat.IsPercentType ?
                $"{stat.DefaultValue * 100f:0.##;-0.##}%" :
                stat.DefaultValue.ToString("0.##;-0.##");

            string bonusValueAsString = stat.IsPercentType ?
                $"{stat.BonusValue * 100f:0.##;-0.##}%" :
                stat.BonusValue.ToString("0.##;-0.##");

            GUI.Label(textRect, $"{stat.DisplayName}: {defaultValueAsString} ({bonusValueAsString})");
            // + Button을 누르면 Stat 증가
            if (GUI.Button(plusButtonRect, "+"))
            {
                if (stat.IsPercentType)
                    stat.DefaultValue += 0.01f;
                else
                    stat.DefaultValue += 1f;
            }

            // - Button을 누르면 Stat 감소
            if (GUI.Button(minusButtonRect, "-"))
            {
                if (stat.IsPercentType)
                    stat.DefaultValue -= 0.01f;
                else
                    stat.DefaultValue -= 1f;
            }

            // 다음 Stat 정보 출력을 위해 y축으로 한칸 내림
            textRect.y += 22f;
            plusButtonRect.y = minusButtonRect.y = textRect.y;
        }
    }
#endif
}