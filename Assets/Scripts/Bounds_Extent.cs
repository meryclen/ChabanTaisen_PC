using UnityEngine;

public class Bounds_Extent : MonoBehaviour
{    
    void Awake()
    {
        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            Bounds b = new Bounds(Vector3.zero, new Vector3(5f, 5f, 5f));
            smr.localBounds = b;
        }
    }
}
