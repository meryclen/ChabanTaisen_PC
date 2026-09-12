using UnityEngine;

public class Fire : MonoBehaviour, ITickable
{
    InputManager inputManager;
    Anim anim;
    GameObject poolManagerGO;
    PoolManager poolManager;

    bool fireEnabled = true;
    float rapidTimer = 0f;
    [SerializeField] float fireEnabledTime = 0.2f;
    Transform gunPivotBone;

    [SerializeField] float offsetx = -0.04f;
    [SerializeField] float offsety = 0.1f;
    [SerializeField] float offsetz = 0.01f;

    [SerializeField] Projectile prefab;
    [SerializeField] SoundManager soundManager;


    public void Initialize(InputManager inputManager, Anim anim)
    {
        this.inputManager = inputManager;
        this.anim = anim;
    }
    void Start()
    {
        poolManagerGO = GameObject.FindWithTag("PoolManager");
        poolManager = poolManagerGO.GetComponent<PoolManager>();

        GameObject gunPivotBoneGO = GameObject.FindWithTag("GunPivotBone");
        gunPivotBone = gunPivotBoneGO.transform;
    }

    public void Tick()
    {
        if (inputManager.isFireButtonHold && fireEnabled && anim.IsAimState)
        {
            rapidTimer = 0f;
            fireEnabled = false;
            RapidFire();
        }
        rapidTimer += Time.deltaTime;
        if (rapidTimer > fireEnabledTime)
        {
            fireEnabled = true;
        }
    }
    public void LateTick() { }
    
    void RapidFire()
    {
        var bullet = poolManager.Get(prefab);
        Vector3 pos = gunPivotBone.position;
        pos += gunPivotBone.right * offsetx +
            gunPivotBone.up * offsety +
            gunPivotBone.forward * offsetz;
        bullet.transform.position = pos;
        bullet.transform.rotation = anim.GunPivot.rotation;
        soundManager.Raise();
    }
}
