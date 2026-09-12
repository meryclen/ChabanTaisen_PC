using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] float distance = 15f;
    [SerializeField] LayerMask lazerpointerLayer;
    LineRenderer lr;
    Transform lazerpointer;
    MaterialPropertyBlock mpb;
    bool isLazerpointer;
    [SerializeField] float lazerpointerOffset = 0.1f;

    public void Initialize()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lazerpointer = transform.Find("Lazerpointer");

        mpb = new MaterialPropertyBlock();
        MeshRenderer mr = lazerpointer.GetComponent<MeshRenderer>();
        mr.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", new Color(10f, 0f, 0f, 0.5f));
        mpb.SetFloat("_EmissionStrength", 1f);
        mr.SetPropertyBlock(mpb);
    }
    void OnEnable()
    {
        isLazerpointer = false;
    }
    void OnDisable()
    {
        isLazerpointer = true;
        lazerpointer.gameObject.SetActive(false);        
    }

    void Update()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;        

        if (Physics.Raycast(
            origin, forward, out RaycastHit hit, distance, lazerpointerLayer))
        {
            lr.SetPosition(0, origin);
            lr.SetPosition(1, hit.point);

            if (!isLazerpointer)
            {
                isLazerpointer = true;
                lazerpointer.gameObject.SetActive(true);
            }
            lazerpointer.position = hit.point + lazerpointerOffset * hit.normal;
            lazerpointer.rotation = transform.rotation;
        }
        else
        {
            lr.SetPosition(0, origin);
            lr.SetPosition(1, origin + forward * distance);

            if (isLazerpointer)
            {
                isLazerpointer = false;
                lazerpointer.gameObject.SetActive(false);
            }
        }
    }
}
