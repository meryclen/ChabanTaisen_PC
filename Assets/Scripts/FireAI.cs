using UnityEngine;

public class FireAI : MonoBehaviour, ITickable
{
    GameObject poolManagerGO;
    PoolManager poolManager;
    StatusAI statusAI;
    MoveAI moveAI;
    ImmovableAI immovableAI;

    [SerializeField] Projectile prefab;
    bool fireEnabled = true;
    float fireTimer = 0f;
    [SerializeField] float fireEnabledTime = 1f;

    [SerializeField] Transform turret;
    [SerializeField] Transform barrel;

    [SerializeField] float yawSpeed = 20f;
    [SerializeField] float pitchSpeed = 10f;
    [SerializeField] float minPitch = -30f;
    [SerializeField] float maxPitch = 45f;
    [SerializeField] float aimThreshold = 1f;
    [SerializeField] float minYaw = -90f;
    [SerializeField] float maxYaw = 90f;
    

    void OnEnable()
    {
        fireEnabled = true;
        fireTimer = fireEnabledTime;
    }
    void Start()
    {
        poolManagerGO = GameObject.FindWithTag("PoolManager");
        poolManager = poolManagerGO.GetComponent<PoolManager>();        
    }
    public void Initialize(StatusAI statusAI, MoveAI moveAI, ImmovableAI immovableAI)
    {
        this.statusAI = statusAI;
        this.moveAI = moveAI;
        this.immovableAI = immovableAI;
    }

    public void Tick()
    {
        if (statusAI.CurStatus != EnemyStatus.Attack)
        {
            fireEnabled = true;
            fireTimer = fireEnabledTime;
            return;
        }
        Aim();
        fireTimer += Time.deltaTime;

        if (fireTimer < fireEnabledTime)
        {
            fireEnabled = false;
        }

        if (fireEnabled)
        {
            Fire();
            fireTimer = 0f;
        }
        fireEnabled = true;        
    }
    public void LateTick()
    {

    }

    void Fire()
    {
        var bullet = poolManager.Get(prefab);
        Vector3 pos = barrel.position;
        Quaternion rot = barrel.rotation;
        bullet.transform.position = pos;
        bullet.transform.rotation = rot;
    }

    void Aim()
    {
        bool yawOK = false;
        bool pitchOK = false;

        Vector3 dir = Vector3.zero;
        
        if (moveAI != null)
        {
            dir = moveAI.TargetDir.normalized;
        }
        if (immovableAI != null)
        {
            dir = immovableAI.TargetDir.normalized;
        }

        Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up);
        Vector3 localYawDir = transform.InverseTransformDirection(flatDir);

        float yaw =
            Mathf.Atan2(
                localYawDir.x,
                localYawDir.z
                ) * Mathf.Rad2Deg;

        if (moveAI == null)
        {
            yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        }
        Quaternion targetLocalYaw = Quaternion.Euler(0f, yaw, 0f);

        Quaternion tmpYaw = turret.localRotation =
            Quaternion.RotateTowards(
                turret.localRotation,
                targetLocalYaw,
                yawSpeed * Time.deltaTime
                );

        if (Mathf.Abs(Quaternion.Angle(tmpYaw, targetLocalYaw)) < aimThreshold) yawOK = true;

        Vector3 verticalDir = Vector3.ProjectOnPlane(dir, turret.right);
        Vector3 localVerticalDir = turret.InverseTransformDirection(verticalDir);

        float pitch =
            -Mathf.Atan2(
                localVerticalDir.y,
                localVerticalDir.z
                ) * Mathf.Rad2Deg;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        Quaternion targetLocalPitch = Quaternion.Euler(pitch, 0f, 0f);

        Quaternion tmpPitch = barrel.localRotation =
            Quaternion.RotateTowards(
                barrel.localRotation,
                targetLocalPitch,
                pitchSpeed * Time.deltaTime
                );
        if (Mathf.Abs(Quaternion.Angle(tmpPitch, targetLocalPitch)) < aimThreshold) pitchOK = true;

        if (!(yawOK && pitchOK)) fireEnabled = false;
    }
}
