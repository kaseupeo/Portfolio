using System;
using UnityEngine;
using UnityEngine.Events;
using Elements = Define.InterfaceElements;

public class TutorialController : MonoBehaviour
{
    public UnityEvent OnInputLKey;
    public UnityEvent OnInputTabKey;
    public UnityEvent OnInputKKey;
    public UnityEvent OnInputPKey;
    public UnityEvent OnInputF1Key;
    
    private void Start()
    {
        Managers.Input.UIPressActionDic[Elements.Quest].started += context => OnInputLKey.Invoke();
        Managers.Input.ChangedWeapon.started += context => OnInputTabKey.Invoke();
        Managers.Input.UIPressActionDic[Elements.Skill].started += context => OnInputKKey.Invoke();
        Managers.Input.UIPressActionDic[Elements.Stat].started += context => OnInputPKey.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            OnInputF1Key.Invoke();
        }
    }
}