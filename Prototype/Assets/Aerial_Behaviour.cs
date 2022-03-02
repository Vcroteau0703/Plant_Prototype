using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aerial_Behaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

    public AnimationClip Aerial_to_Fall, Implied_Jump, Normal_Aerial;
    protected AnimatorOverrideController animatorOverrideController;

    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(layerIndex);

    //    animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
    //    animator.runtimeAnimatorController = animatorOverrideController;

    //    if (transition.IsName("IDLE -> AERIAL") || (transition.IsName("WALK -> AERIAL")) 
    //    {
    //        animatorOverrideController["AERIAL"] = 
    //    }
    //    else if (transition.IsName("")) { }

    //    //Debug.Log(animator.GetAnimatorTransitionInfo(layerIndex).IsName("IDLE -> AERIAL"));
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.Log(animator.GetAnimatorTransitionInfo(layerIndex).IsName("IDLE -> AERIAL"));

    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
