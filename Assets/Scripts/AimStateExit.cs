using UnityEngine;

public class AimStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var receiver = animator.GetComponent<Anim>();
        receiver.OnAimStateExit();
    }
}
