using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] float r;
    [SerializeField] float g;
    [SerializeField] float b;
    [SerializeField] float a;
    [SerializeField] float e;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        var coloring = new Coloring(mr);
        coloring.ColorSetting(r, g, b, a, e);
    }
}
