using System;
using System.Collections.Generic;

[Serializable]
public abstract class EffectAction : ICloneable
{
    // 효과가 시작될 때 호출되는 함수
    public virtual void Start(Effect effect, Creature user, Creature target, int level, float scale) { }
    
    // 실제 효과를 구현하는 함수
    public abstract bool Apply(Effect effect, Creature user, Creature target, int level, int stack, float scale);
    
    // 효과가 종료될 때 호출되는 함수
    public virtual void Release(Effect effect, Creature user, Creature target, int level, float scale) { }

    // 효과의 스택이 바뀌었을 때 호출되는 함수
    public virtual void OnEffectStackChanged(Effect effect, Creature user, Creature target, int level, int stack, float scale) { }
    
    // 
    protected virtual IReadOnlyDictionary<string, string> GetStringByKeywordDic(Effect effect) => null;

    
    public string BuildDescription(Effect effect, string description, int stackActionIndex, int stack, int effectIndex)
    {
        var stringByKeywordDic = GetStringByKeywordDic(effect);

        if (stringByKeywordDic == null)
            return description;

        if (stack == 0)
            description = TextReplacer.Replace(description, "effectAction", stringByKeywordDic, effectIndex.ToString());
        else
            description = TextReplacer.Replace(description, "effectAction", stringByKeywordDic,
                $"{stackActionIndex}.{stack}.{effectIndex}");

        return description;
    }
    
    public abstract object Clone();
}