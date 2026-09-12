using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    [SerializeField] MeshRenderer mr;
    void Start()
    {
        var coloring = new Coloring(mr);
        var mpb = new MaterialPropertyBlock();
        coloring.ColorSetting(5f, 5f, 0f, 1f, 1f);
    }
}
