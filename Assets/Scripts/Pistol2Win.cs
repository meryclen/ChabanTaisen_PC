using UnityEngine;

public class Pistol2Win : MonoBehaviour
{
    [SerializeField] float x_on = -0.03f;
    [SerializeField] float y_on = 0.07f;
    [SerializeField] float z_on = 0.01f;
    [SerializeField] float x_off = -90f;
    [SerializeField] float y_off = 100f;
    [SerializeField] float z_off = 0f;

    Transform rightHand;
    Vector3 localPos;

    void Start()
    {
        rightHand = transform.root.GetComponent<Animator>()
            .GetBoneTransform(HumanBodyBones.RightHand);
        localPos = new Vector3(x_on, y_on, z_on);
    }
    
    void LateUpdate()
    {
        transform.localPosition = localPos;
        transform.rotation = rightHand.rotation * Quaternion.Euler(x_off, y_off, z_off);
    }
}
