using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged_ChoosingTile : StateMachineBehaviour
{
    public delegate void Ranged_ChoosingTileUpdateDelegate(Animator animator);
    public static Ranged_ChoosingTileUpdateDelegate OnRanged_ChoosingTileUpdate;
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnRanged_ChoosingTileUpdate?.Invoke(animator);
    }

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}
}
