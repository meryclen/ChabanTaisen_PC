using UnityEngine;

public class AimStateEnter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAimFinished", false);
        var receiver = animator.GetComponent<Anim>();
        receiver.OnAimStateEnter();
    }
}
