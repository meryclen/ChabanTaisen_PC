using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class GroundAI : GroundMoter
{
    [SerializeField] MoveAI moveAI;
    [SerializeField] StatusAI statusAI;
    [SerializeField] DamageAI damageAI;

    [SerializeField] Transform[] probePoints;
    public GroundHit[] probes;
    [SerializeField] float rayOffset2 = 0.2f;


    protected override void Awake()
    {
        probes = new GroundHit[probePoints.Length];
        for (int i = 0; i < probePoints.Length; i++)
        {
            probes[i] = new GroundHit();
        }
    }
    public void Initialize(MoveAI moveAI)
    {
        this.moveAI = moveAI;
    }

    public override void Tick()
    {
        Vector3 prevPos = transform.position;

        desiredV = Vector3.zero;

        Vector3 origin = prevPos;
        bool checkDown = false;
        Collider col = null;
        RaycastHit hit = default;
        
        checkDown = Physics.Raycast(
            origin, Vector3.down, out hit, settings.rayLength, settings.layerMask
            );
        col = hit.collider;

        if (checkDown)
        {
            GravityUpdate(ref IsGrounded, checkDown);

            Vector3 moveDir = Vector3.ProjectOnPlane(
                Move,
                hit.normal
                ).normalized * Move.magnitude;

            //斜面滑り
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            Vector3 slideV = Vector3.zero;
            if (slopeAngle > settings.slopeSlideLimit)
            {
                slideV = Vector3.ProjectOnPlane(
                Vector3.down * settings.slideSpeed,
                hit.normal
                );
            }            

            desiredV = moveDir + slideV;

            if (hit.point.y - prevPos.y > desiredV.y * Time.deltaTime)
            {
                desiredV.y = hit.point.y - prevPos.y;
            }
        }
        else
        {
            GravityUpdate(ref IsGrounded, checkDown);
            desiredV.x = Move.x * settings.decreaseValue;
            desiredV.z = Move.z * settings.decreaseValue;
            desiredV.y = -gravityVelocity;
        }
        
        //吹っ飛び
        if (!isBlow &&
            gravityDamageInfo.DamageType != DamageType.None &&
            gravityDamageInfo.DamageType != DamageType.Falling)
        {
            isBlow = true;

            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();
            linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                destroyToken, cts.Token);
            linkedToken = linkedCts.Token;

            blowDist = blowDistMaxDelta * gravityDamageInfo.ShockPower;
            moveAI.Agent.enabled = false;

            BlowAsync(linkedToken).Forget(ex => Debug.LogError($"BlowAsyncで例外エラー: {ex}"));
        }

        desiredV += blowV;
        Vector3 desiredDir = desiredV.normalized;
        float desiredDist = desiredV.magnitude * Time.deltaTime;

        float tmpY = prevPos.y + settings.hitCheckOriginOffsetY;
        Vector3 tmp = prevPos;
        tmp.y = tmpY;

        bool hitCheck = Physics.SphereCast(tmp, settings.spherecastRadius, desiredDir,
            out RaycastHit hitSphere, desiredDist + settings.spherecastOffset, settings.layerMask);
        Vector3 hitSphereNormal = hitSphere.normal;

        if (hitCheck)
        {
            if (gravityVelocity > settings.deadValue)
            {
                statusAI.Die(DeadType.FallClash);
            }

            Vector3 hitV = desiredDir * Mathf.Max(0, hitSphere.distance - settings.skinWidth);
            Vector3 remainV = desiredV - hitV;
            float upDot = Vector3.Dot(hitSphereNormal, Vector3.up);

            if (upDot > settings.wallThreshold)
            {
                remainV = Vector3.ProjectOnPlane(remainV, hitSphereNormal);
            }
            else
            {
                remainV = Vector3.zero;
            }

            desiredV = hitV + remainV;
        }

        Move = desiredV;

        SetProbePoints();
    }

    void GravityUpdate(ref bool isGrounded, bool isCheckDown)
    {
        if (isCheckDown)
        {
            if (!isGrounded)
            {
                isGrounded = true;
                if (gravityVelocity > settings.deadValue)
                {
                    if (statusAI.CurStatus != EnemyStatus.Dead)
                    {
                        statusAI.Die(DeadType.FallClash);
                    }
                    else
                    {
                        damageAI.Damage(gravityDamageInfo);
                    }
                }
                else if (gravityVelocity > settings.damageValue)
                {
                    //落下ダメージ                    
                    gravityDamageInfo.DamageType = DamageType.Falling;
                    gravityDamageInfo.AttackPower = 20;
                    damageAI.Damage(gravityDamageInfo);
                }
            }
            gravityVelocity = 0f;
        }
        else
        {
            isGrounded = false;
            gravityVelocity += settings.gravityAcceleration * Time.deltaTime;
            gravityVelocity = Mathf.Clamp(gravityVelocity, 0f, settings.gravityVelocityMax);

            if (transform.position.y < -25f) statusAI.Die(DeadType.DropOut);
        }
    }

    protected override async UniTask BlowAsync(CancellationToken ct)
    {
        try
        {
            while (blowDist > 0.01f)
            {
                blowDist = Mathf.Lerp(
                    blowDist,
                    0f,
                    10f * Time.deltaTime);

                blowV = -gravityDamageInfo.DamageHitNormal * blowDist;

                await UniTask.Yield(cancellationToken: ct);
            }

            blowV = Vector3.zero;
            isBlow = false;
            gravityDamageInfo.DamageType = DamageType.None;
        }
        catch (OperationCanceledException)
        {
            Log.D("groundMoter.Tickでキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    void SetProbePoints()
    {
        for (int i = 0; i < probePoints.Length; i++)
        {
            Vector3 origin = probePoints[i].position;
            origin.y += rayOffset2;

            if (Physics.Raycast(
                origin, Vector3.down,
                out RaycastHit hit,
                settings.rayLength,
                settings.layerMask))
            {
                probes[i].hit = true;
                probes[i].point = hit.point;
                probes[i].normal = hit.normal;
            }
            else probes[i].hit = false;
        }
    }
}
