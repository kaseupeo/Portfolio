using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameterSetter : StateMachineBehaviour
{
    private enum State { Enter, Exit }

    [SerializeField] private State state;
    [SerializeField] private string parameterName;
    [SerializeField] private bool isOn;

    private int _hash = int.MinValue;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_hash == int.MinValue)
            _hash = Animator.StringToHash(parameterName);

        if (state == State.Enter)
            animator.SetBool(_hash, isOn);
    }


    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == State.Exit)
            animator.SetBool(_hash, isOn);
    }
}
