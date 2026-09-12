using UnityEngine;

public class FingerCtrlLeft : FingerCtrl
{
    protected override void Grip()
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            for (int j = 0; j < fingers[i].fingerConstraints.Length; j++)
            {
                fingers[i].fingerConstraints[j].pitch +=
                    speedBase * fingers[i].fingerConstraints[j].rate;
                fingers[i].fingerConstraints[j].pitch = Mathf.Clamp(
                    fingers[i].fingerConstraints[j].pitch,
                    fingers[i].fingerConstraints[j].min,
                    fingers[i].fingerConstraints[j].max);
                fingers[i].fingerConstraints[j].target.localRotation =
                    Quaternion.AngleAxis(fingers[i].fingerConstraints[j].pitch, Vector3.right);
            }
        }
    }

    protected override void LateUpdate()
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            fingers[i].targetBase.rotation = transform.rotation * fingers[i].offsetRot;
        }
        if (anim != null && anim.IsAimState) DefaultGrip();
        else if (isGrip) Grip();
    }
}
