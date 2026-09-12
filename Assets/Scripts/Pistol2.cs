using UnityEngine;

public class Pistol2 : MonoBehaviour, ITickable
{
    Anim anim;

    [SerializeField] float x_on;
    [SerializeField] float y_on;
    [SerializeField] float z_on;
    [SerializeField] float x_off;
    [SerializeField] float y_off;
    [SerializeField] float z_off;

    Transform rightHand;
    [SerializeField] Transform gunPivot;
    Vector3 localPos;

    public void Initialize(Anim anim)
    {
        this.anim = anim;
    }
    void Start()
    {
        Transform rootTrans = transform.root;
        rightHand = anim.Animator.GetBoneTransform(HumanBodyBones.RightHand);
        localPos = new Vector3(x_on, y_on, z_on);
    }

    public void Tick() { }
    public void LateTick()
    {
        transform.localPosition = localPos;

        if (anim.RightArmIK.weight == 1)
        {
            transform.rotation = gunPivot.rotation;
        }
        else
        {
            transform.rotation = rightHand.rotation * Quaternion.Euler(x_off, y_off, z_off);
        }
    }
}
