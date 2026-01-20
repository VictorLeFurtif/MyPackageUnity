using UnityEngine;
using UnityEngine.Animations;

public class ResetBool : StateMachineBehaviour
{
    public string m_isInteractingBool;
    public bool m_isInteractingStatus;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
        AnimatorControllerPlayable controller)
    {
        animator.SetBool(m_isInteractingBool,m_isInteractingStatus);
    }
}
