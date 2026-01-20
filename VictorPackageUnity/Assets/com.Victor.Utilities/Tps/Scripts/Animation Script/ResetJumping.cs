using UnityEngine;

public class ResetJumping : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsJumping",false);
    }
}
