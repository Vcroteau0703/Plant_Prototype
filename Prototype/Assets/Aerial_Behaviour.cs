using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aerial_Behaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    [Header("JUMPING")]
    public AnimationClip Implied_Wall_Jump, Implied_Jump;
    [Header("AERIAL")]
    public AnimationClip Normal_Aerial;
    [Header("FALLING")]
    public AnimationClip Fall, Fall_Transition;
    [Header("GLIDING")]
    public AnimationClip Glide;

    protected AnimatorOverrideController controller;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(layerIndex);

        controller = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = controller;

        if (transition.IsName("GROUND -> AIR"))
        {
            controller["Aerial"] = Implied_Jump;
        }
        else if(transition.IsName("WALL -> AIR"))
        {
            controller["Aerial"] = Implied_Wall_Jump;
        }

        //Debug.Log(animator.GetAnimatorTransitionInfo(layerIndex).IsName("IDLE -> AERIAL"));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool isGlide = animator.GetBool("GLIDE");
        bool isAerial = animator.GetBool("AERIAL");
        float velocity_Y = animator.GetFloat("VELOCITY_Y");

        if (isAerial)
        {
            if (controller["Aerial"] == Normal_Aerial && stateInfo.normalizedTime < 1.5f) { return; }

            if (velocity_Y < 0)
            {
                controller["Aerial"] = Fall;
            }
            else
            {
                controller["Aerial"] =  Normal_Aerial;
            }

        }
        else if(isGlide)
        {
            controller["Aerial"] = Glide;
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
    //    Debug.Log(animator.GetAnimatorTransitionInfo(layerIndex).IsName("IDLE -> AERIAL"));

    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
