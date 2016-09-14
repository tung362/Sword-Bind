using UnityEngine;
using System.Collections;

[System.Serializable]
public struct IntVector2
{
    public int x, y;
    public IntVector2(int x1, int y1) { x = x1; y = y1; }
}

public class RandomValueOnEndOfAnimation : StateMachineBehaviour
{
    [Header("The Type Of Edit (Only have one toggled)")]
    public bool IsInt = false;
    public bool IsFloat = false;

    [Header("The Random Value Range(Only use the correct type)")]
    public IntVector2 IntRange = new IntVector2(0, 0);
    public Vector2 FloatRange = new Vector2(0, 0);

    [Header("Name of the paremeter that is going to be edited")]
    public string ParameterName;

    [Header("Settings")]
    private bool RunOnce = true;
    public float RestrictedNumber = 0;
    private float StartingAnimationEndValue = 1;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StartingAnimationEndValue = 1;
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= StartingAnimationEndValue)
        {
            if (RunOnce == true)
            {
                if (IsInt)
                {
                    int result = Random.Range(IntRange.x, IntRange.y + 1);
                    animator.SetInteger(ParameterName, result);
                    if ((int)RestrictedNumber == result) StartingAnimationEndValue += 1;
                }
                if (IsFloat)
                {
                    float result = Random.Range(FloatRange.x, FloatRange.y);
                    animator.SetFloat(ParameterName, result);
                    if (RestrictedNumber == result) StartingAnimationEndValue += 1;
                }
                RunOnce = false;
            }
        }
        else RunOnce = true;
    }
}
