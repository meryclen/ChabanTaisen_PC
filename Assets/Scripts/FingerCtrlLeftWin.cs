using UnityEngine;

public class FingerCtrlLeftWin : FingerCtrl
{
    [SerializeField] bool isGrip1;
    [SerializeField] bool isGrip2;
    [SerializeField] float targetPitch;
    [SerializeField] float lerpSpeed;

    protected override void Grip()
    {

    }

    protected override void LateUpdate()
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            fingers[i].targetBase.rotation = transform.rotation * fingers[i].offsetRot;
        }
        if (isGrip1) GripFunc(targetPitch, lerpSpeed);
        if (isGrip2) GripFunc(targetPitch, lerpSpeed);
    }

    public void Grip1()
    {
        targetPitch = -15f;
        lerpSpeed = 0.5f;
        isGrip1 = true;
    }

    void GripFunc(float targetPitch, float lerpSpeed)
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            for (int j = 0; j < fingers[i].fingerConstraints.Length; j++)
            {
                fingers[i].fingerConstraints[j].pitch =
                    Mathf.Lerp(
                        fingers[i].fingerConstraints[j].pitch,
                        targetPitch,
                        lerpSpeed * Time.deltaTime);
                fingers[i].fingerConstraints[j].pitch = Mathf.Clamp(
                    fingers[i].fingerConstraints[j].pitch,
                    fingers[i].fingerConstraints[j].min,
                    fingers[i].fingerConstraints[j].max);
                fingers[i].fingerConstraints[j].target.localRotation =
                    Quaternion.AngleAxis(fingers[i].fingerConstraints[j].pitch, Vector3.right);
            }
        }
    }

    public void Grip2()
    {
        targetPitch = 90f;
        lerpSpeed = 1f;
        isGrip1 = false;
        isGrip2 = true;
    }
}
