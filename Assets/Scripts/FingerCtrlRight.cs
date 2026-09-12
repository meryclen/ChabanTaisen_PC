public class FingerCtrlRight : FingerCtrl
{
    protected override void Grip()
    {
        
    }

    protected override void LateUpdate()
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            fingers[i].targetBase.rotation = transform.rotation * fingers[i].offsetRot;
        }
        DefaultGrip();
    }
}
