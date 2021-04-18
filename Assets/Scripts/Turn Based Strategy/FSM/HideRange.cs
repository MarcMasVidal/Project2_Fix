using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRange : StateMachineBehaviour
{
    public delegate void HideRangeEnterDelegate();
    public static HideRangeEnterDelegate OnHideRangeEnter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnHideRangeEnter?.Invoke();
    }
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}
}
