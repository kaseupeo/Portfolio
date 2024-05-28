using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SkillPrecedingAction : ICloneable
{
    public virtual void Start(Skill skill) { }
    public abstract bool Run(Skill skill);
    public virtual void Release(Skill skill) { }

    protected virtual IReadOnlyDictionary<string, string> GetStringByKeywordDic() => null;

    public virtual string BuildDescription(string description)
        => TextReplacer.Replace(description, "precedingAction", GetStringByKeywordDic());

    public abstract object Clone();
}
