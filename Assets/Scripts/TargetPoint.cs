using UnityEngine;

public class TargetPoint : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] float radius = 0.5f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}
