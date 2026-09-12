using UnityEngine;

public class DamageStateExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var receiver = animator.GetComponent<Anim>();
        receiver.OnDamageStateExit();
    }
}
