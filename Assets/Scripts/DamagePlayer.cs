using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class DamagePlayer : DamageController
{
    Anim anim;    

    CancellationToken destroyToken;
    CancellationTokenSource flashCts;

    [SerializeField] SoundManager soundManager;
    SoundPlayer soundPlayer;

    
    public override void Initialize(Enemy enemy, MoveAI moveAI, GroundMoter groundMoter, Anim anim)
    {
        base.Initialize(null, null, groundMoter, null);
        this.anim = anim;
    }    
    protected override void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
        if (soundManager is SoundPlayer sp) soundPlayer = sp;
    }
    protected override void OnEnable()
    {

    }
    protected override void OnDisable()
    {
        flashCts?.Cancel();
    }
    protected override void OnDestroy()
    {
        flashCts?.Cancel();
        flashCts?.Dispose();
    }

    public override void Damage(DamageInfo damageInfo)
    {
        base.Damage(damageInfo);

        if (statusController is StatusPlayer s)
        {
            if (statusController.IsHpZero)
            {
                statusController.Die(DeadType.HpZero);
                s.RaiseDamage(PlayerStatus.Dead, 0);
                soundPlayer.FallDown();
                return;
            }

            if (damageInfo.DamageType == DamageType.Falling &&
                s.PlayerStatus == PlayerStatus.Fine)
            {
                Vector3 tmp = transform.position;
                tmp.y -= 0.5f;
                transform.position = tmp;
                anim.ArmIkOff();
                anim.FootIkOff();
                anim.Animator.SetInteger("damageType", 4);
                anim.Animator.SetTrigger("damage");
                s.RaiseDamage(PlayerStatus.FallDown, 200);
                soundPlayer.FallDown();
            }
            else if (damageInfo.ShockPower > 200)
            {
                anim.Animator.SetInteger("damageType", 3);
                anim.Animator.SetTrigger("damage");
                anim.IsRecoverIkCancel = true;
                s.RaiseDamage(PlayerStatus.FallDown, 200);
                soundPlayer.FallDown();
            }
            else if (damageInfo.ShockPower > 10)
            {
                if (groundMoter.gravityDamageInfo.CanAnimOverride)
                {
                    if (s.PlayerStatus != PlayerStatus.Recover)
                        s.PlayerStatus = PlayerStatus.FallDown;
                    int rnd = Random.Range(0, 2);
                    anim.ArmIkOff();
                    anim.FootIkOff();
                    anim.Animator.SetInteger("damageRnd", rnd);
                    anim.Animator.SetInteger("damageType", 1);
                    if (anim.IsAimState) anim.Animator.SetTrigger("damageAim");
                    else anim.Animator.SetTrigger("damage");
                    s.RaiseDamage(PlayerStatus.FallDown, 10);
                    soundPlayer.Damage();
                }
            }
            else
            {
                if (groundMoter.gravityDamageInfo.CanAnimOverride)
                {
                    anim.Animator.SetInteger("damageType", 1);
                    if (anim.IsAimState) anim.Animator.SetTrigger("damageAim");
                    else anim.Animator.SetTrigger("damage");
                    s.RaiseDamage(PlayerStatus.Fine, 1);
                    soundPlayer.Damage();
                }
            }
        }

        flashCts?.Cancel();
        flashCts?.Dispose();

        flashCts = new CancellationTokenSource();

        var linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                destroyToken, flashCts.Token);

        flashController.FlashAsync(linkedCts).Forget(
            ex => Debug.LogError($"FlashAsync‚Å—áŠOƒGƒ‰[: {ex}"));
    }
}
