using UnityEngine;

public abstract class DamageController : MonoBehaviour
{
    [SerializeField] SurfaceType surfaceType;
    public SurfaceType SurfaceType => surfaceType;

    [SerializeField] protected StatusController statusController;
    [SerializeField] protected FlashController flashController;
    protected GroundMoter groundMoter;

    public virtual void Initialize(Enemy enemy, MoveAI moveAI, GroundMoter groundMoter, Anim anim)
    {
        this.groundMoter = groundMoter;        
    }
    protected virtual void Awake()
    {

    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {

    }
    protected virtual void OnDestroy()
    {

    }

    public virtual void Damage(DamageInfo damageInfo)
    {
        statusController.Hp -= damageInfo.AttackPower;

        if (groundMoter != null)
        {
            if (damageInfo.ShockPower >= groundMoter.gravityDamageInfo.CurShockPower)
            {
                groundMoter.gravityDamageInfo.CurShockPower = damageInfo.ShockPower;
                groundMoter.gravityDamageInfo.CanAnimOverride = true;
            }
            else
            {
                groundMoter.gravityDamageInfo.CanAnimOverride = false;
            }

            groundMoter.gravityDamageInfo.ShockPower = damageInfo.ShockPower;
            groundMoter.gravityDamageInfo.DamageType = damageInfo.DamageType;
            groundMoter.gravityDamageInfo.DamageHitNormal = damageInfo.DamageHitNormal;
        }

        statusController.RaiseHpChanged();
    }
}
