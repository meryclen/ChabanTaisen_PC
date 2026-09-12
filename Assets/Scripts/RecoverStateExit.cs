using UnityEngine;

public class RecoverStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var receiver = animator.GetComponent<Anim>();
        receiver.OnRecoverStateExit();
    }
}
