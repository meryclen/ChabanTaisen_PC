using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] FingerCtrlLeftWin left;

    public void Grip1()
    {
        left.Grip1();
    }
    public void Grip2()
    {
        left.Grip2();
    }
}
