using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_Behaviour : StateMachineBehaviour
{
    [Header("LANDING")]
    public AnimationClip Landing;
    [Header("MOVING")]
    public AnimationClip Moving, Move_Trasition;
    [Header("IDLE")]
    public AnimationClip Idle;

    public Animation transition;

    public float delay;

    protected AnimatorOverrideController controller;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(layerIndex);

        controller = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = controller;

        if (transition.IsName("AIR -> GROUND"))
        {
            controller["Idle"] = Landing;
            delay = 0;
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int DIR_X = animator.GetInteger("DIR_X");

        controller["Idle"] = DIR_X != 0 ? Moving : Idle;

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
