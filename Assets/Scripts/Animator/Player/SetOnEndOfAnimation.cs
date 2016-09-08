using UnityEngine;
using System.Collections;

public class SetOnEndOfAnimation : StateMachineBehaviour
{
    [Header("The Type Of Edit (Only have one toggled)")]
    public bool IsBool = false;
    public bool IsInt = false;
    public bool IsFloat = false;

    [Header("The Value To Edit To (Only use the correct type)")]
    public bool BoolValue = false;
    public int IntValue = 0;
    public float FloatValue = 0;

    [Header("Name of the paremeter that is going to be edited")]
    public string ParameterName;

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1)
        {
            if (IsBool) animator.SetBool(ParameterName, BoolValue);
            if (IsInt) animator.SetInteger(ParameterName, IntValue);
            if (IsFloat) animator.SetFloat(ParameterName, FloatValue);
        }
    }
}
