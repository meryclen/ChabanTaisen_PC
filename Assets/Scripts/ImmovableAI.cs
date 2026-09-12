using UnityEngine;

public class ImmovableAI : MonoBehaviour, ITickable
{
    Transform target;
    public Transform Target { set => target = value; }
    [SerializeField] Transform barrel;

    StatusAI statusAI;

    Vector3 targetDir;
    public Vector3 TargetDir => targetDir;
    [SerializeField] Vector3 targetOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] Vector3 targetOffset2;

    float timer;

    [SerializeField] float sqrAttackThreshold = 100f;


    public void Initialize(StatusAI statusAI)
    {
        this.statusAI = statusAI;
    }

    public void Tick()
    {
        timer += Time.deltaTime;

        if (timer > 0.5f)
        {
            timer = 0f;
            TargetDirCalc();

            if (statusAI.CurStatus != EnemyStatus.Attack &&
                targetDir.sqrMagnitude < sqrAttackThreshold)
            {
                statusAI.CurStatus = EnemyStatus.Attack;
            }
            else if (statusAI.CurStatus == EnemyStatus.Attack &&
                targetDir.sqrMagnitude > sqrAttackThreshold)
            {
                statusAI.CurStatus = EnemyStatus.Idle;
            }
        }
    }
    public void LateTick() { }

    void TargetDirCalc()
    {
        Vector3 rndV = Vector3.zero;
        int rndRight = Random.Range(-4, 5);
        int rndUp = Random.Range(-4, 5);

        rndV += transform.right * rndRight * 0.1f + transform.up * rndUp * 0.1f;
        targetDir = target.position +
            targetOffset +
            targetOffset2 -
            barrel.position + rndV;
    }
}
