using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorParameterType
{
    Bool,
    Trigger
}

[System.Serializable]
public struct AnimatorParameter
{
    public AnimatorParameterType type;
    public string name;

    private int _hash;

    public bool IsValid => !string.IsNullOrEmpty(name);

    public int Hash
    {
        get
        {
            if (_hash == 0 && IsValid)
                _hash = Animator.StringToHash(name);
            return _hash;
        }
    }
}
