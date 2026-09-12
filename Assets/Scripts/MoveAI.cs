using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MoveAI : MonoBehaviour, ITickable
{
    NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    Transform target;
    public Transform Target { set => target = value; }
    [SerializeField] Transform barrel;

    StatusAI statusAI;
    GroundMoter groundMoter;
    public Vector3 moveDir;

    bool isGreater; //îÌíeÇ∑ÇÈÇ∆çıìGîÕàÕÇ™çLÇ≠Ç»ÇÈ
    public bool IsGreater { get => isGreater; set => isGreater = value; }
    [SerializeField] float sqrAttackThreshold = 200f; //!isGreateréûÇÃçıìGîÕàÕ
    [SerializeField] float sqrAttackThresholdGreater = 500f; //isGreateréûÇÃçıìGîÕàÕ

    GameObject targetPointsRoot;
    [SerializeField] Transform[] targetPoints;
    [SerializeField] float patrolSpeed = 1f;
    [SerializeField] float attackSpeed = 3.5f;
    
    int targetIndex;
    public int TargetIndex { get => targetIndex; set => targetIndex = value; }
    public int TargetIndexNum => targetPoints.Length;

    Vector3 targetDir;
    public Vector3 TargetDir => targetDir;
    [SerializeField] Vector3 targetOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] Vector3 targetOffset2;

    float timer;

    [SerializeField] Stage stage;


    public void Initialize(StatusAI statusAI, GroundMoter groundMoter)
    {
        this.statusAI = statusAI;
        this.groundMoter = groundMoter;
    }
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.isStopped = true;

        targetPointsRoot = GameObject.FindWithTag("TargetPoints");
        targetPoints =
            targetPointsRoot.transform
            .GetComponentsInChildren<Transform>()
            .Where(t => t != targetPointsRoot.transform)
            .ToArray();

        for (int i = 0; i < targetPoints.Length; i++)
        {
            if (NavMesh.SamplePosition(
                targetPoints[i].position,
                out NavMeshHit hit,
                1f,
                NavMesh.AllAreas))
            {
                targetPoints[i].position = hit.position;
            }
        }
        stage = GameObject.FindWithTag("Stage").GetComponent<Stage>();
    }
    void OnEnable()
    {
        agent.enabled = true;
        agent.isStopped = false;
        isGreater = false;
        
        // enemy0ÇÃspawn
        if (NavMesh.SamplePosition(
            stage.SpawnPoints0[stage.SpawnIndex0].position,
            out NavMeshHit hit,
            1f,
            NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            transform.position = hit.position;
            int tmp = stage.SpawnIndex0 + 1;
            stage.SpawnIndex0 = tmp % stage.SpawnPoints0Length;
        }

        isGreater = false;        
    }

    public void Tick()
    {
        timer += Time.deltaTime;

        if (!agent.enabled) return;

        if (timer > 0.5f) //0.5fïbÇ≤Ç∆Ç…targetDirÇçXêV
        {
            timer = 0f;
            TargetDirCalc();

            if (statusAI.CurStatus != EnemyStatus.Attack &&
                targetDir.sqrMagnitude < (isGreater ? sqrAttackThresholdGreater : sqrAttackThreshold))
            {
                agent.isStopped = false;
                statusAI.CurStatus = EnemyStatus.Attack;
            }
            else if (statusAI.CurStatus == EnemyStatus.Attack &&
                targetDir.sqrMagnitude > (isGreater ? sqrAttackThresholdGreater : sqrAttackThreshold))
            {
                agent.isStopped = false;
                statusAI.CurStatus = EnemyStatus.Patrol;
            }

            if (statusAI.CurStatus == EnemyStatus.Patrol)
            {
                if (agent.isStopped) agent.isStopped = false;

                agent.speed = patrolSpeed;

                bool result = agent.SetDestination(targetPoints[targetIndex].position);
            }
            else if (statusAI.CurStatus == EnemyStatus.Attack)
            {
                if (agent.isStopped) agent.isStopped = false;

                agent.speed = attackSpeed;

                bool result = agent.SetDestination(target.position);
            }
            else if (statusAI.CurStatus == EnemyStatus.Idle)
            {
                if (!agent.isStopped) agent.isStopped = true;
            }
        }
        moveDir = agent.desiredVelocity.normalized;
        Vector3 tmp = moveDir * agent.speed;
        groundMoter.Move = tmp;

        //Debugóp
        Log.D(
$@"
desiredVelocity={agent.desiredVelocity}
move={groundMoter.Move}
nextPosition={agent.nextPosition}
position={transform.position}
hasPath={agent.hasPath}
isOnNavMesh={agent.isOnNavMesh}
remaining={agent.remainingDistance}
");
    }
    public void LateTick() { }

    void TargetDirCalc()
    {
        Vector3 rndV = Vector3.zero;
        int rndRight = Random.Range(-4, 5);
        int rndUp = Random.Range(-4, 5);

        rndV += transform.right * rndRight * 0.1f + transform.up * rndUp * 0.1f;
        Log.D($"target: {target}");
        targetDir = target.position +
            targetOffset +
            targetOffset2 -
            barrel.position + rndV;
    }
}
