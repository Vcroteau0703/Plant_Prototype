using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall_Behaviour : StateMachineBehaviour
{
    public AnimationClip Aerial_to_Fall, Normal_Fall;
    protected AnimatorOverrideController animatorOverrideController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(layerIndex);

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        if (transition.IsName("AERIAL -> FALL") || transition.IsName("CLING -> FALL") || transition.IsName("GLIDE -> FALL"))
        {
            animatorOverrideController["Fall"] = Aerial_to_Fall;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("FALL"))
        {
            if (animatorOverrideController["Fall"] == Aerial_to_Fall && stateInfo.normalizedTime >= 1.5f)
            {
                animatorOverrideController["Fall"] = Normal_Fall;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
