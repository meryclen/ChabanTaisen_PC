using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class GroundPlayer : GroundMoter
{
    [SerializeField] StatusPlayer statusPlayer;
    [SerializeField] DamagePlayer damagePlayer;
    bool[] checkRays;
    RaycastHit[] hitRays;


    protected override void Awake()
    {
        checkRays = new bool[settings.raysNum];
        hitRays = new RaycastHit[settings.raysNum];
    }

    public override void Tick()
    {
        Vector3 prevPos = transform.position;

        desiredV = Vector3.zero;

        Vector3 origin = prevPos;

        bool checkDown = false;
        Collider col = null;
        RaycastHit hit = default;
        
        if (statusPlayer.PlayerStatus == PlayerStatus.Dead)
        {
            origin.y += settings.rayOffsetDead;
            Log.R(origin, Vector3.down * settings.rayLengthDead, Color.red);
            checkDown = Physics.Raycast(
                origin, Vector3.down, out hit, settings.rayLengthDead, settings.layerMask);
        }
        else
        {
            origin.y += settings.rayOffset;
            Log.R(origin, Vector3.down * settings.rayLength, Color.blue);
            checkDown = Physics.Raycast(
                origin, Vector3.down, out hit, settings.rayLength, settings.layerMask);
        }
        col = hit.collider;

        if (checkDown)
        {
            if ((settings.goalMask & (1 << col.gameObject.layer)) != 0)
            {
                if (statusPlayer != null)
                {
                    if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
                    {
                        statusPlayer.Win();
                        return;
                    }
                }
            }

            GravityUpdate(ref IsGrounded, checkDown);

            //斜面に移動速度ベクトルを沿わせる
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
                    hit.normal);
            }
            if (statusPlayer.PlayerStatus == PlayerStatus.FallDown ||
                statusPlayer.PlayerStatus == PlayerStatus.Recover)
                moveDir = Vector3.zero;

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
            
            Vector3 pushV = Vector3.zero;
            Vector3 transformForward = transform.forward;
            Vector3 transformRight = transform.right;
            Vector3 pushOrigin = origin;

            if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
            {
                pushOrigin.y += settings.hitPushCheckOriginOffsetY;

                checkRays[0] =
                    Physics.Raycast(
                        pushOrigin,
                        transformForward,
                        out hitRays[0],
                        settings.raysLength,
                        settings.layerMask);
                checkRays[1] =
                    Physics.Raycast(
                        pushOrigin,
                        -transformForward,
                        out hitRays[1],
                        settings.raysLength,
                        settings.layerMask);
                checkRays[2] =
                    Physics.Raycast(
                        pushOrigin,
                        transformRight,
                        out hitRays[2],
                        settings.raysLength,
                        settings.layerMask);
                checkRays[3] =
                    Physics.Raycast(
                        pushOrigin,
                        -transformRight,
                        out hitRays[3],
                        settings.raysLength,
                        settings.layerMask);
            }
            else
            {
                pushOrigin.y += settings.hitPushCheckOriginOffsetY_Dead;

                checkRays[0] =
                    Physics.Raycast(
                        pushOrigin,
                        transformForward,
                        out hitRays[0],
                        settings.raysLengthDead,
                        settings.layerMask);
                checkRays[1] =
                    Physics.Raycast(
                        pushOrigin,
                        -transformForward,
                        out hitRays[1],
                        settings.raysLengthDead,
                        settings.layerMask);
                checkRays[2] =
                    Physics.Raycast(
                        pushOrigin,
                        transformRight,
                        out hitRays[2],
                        settings.raysLengthDead,
                        settings.layerMask);
                checkRays[3] =
                    Physics.Raycast(
                        pushOrigin,
                        -transformRight,
                        out hitRays[3],
                        settings.raysLengthDead,
                        settings.layerMask);
            }

            bool checkRaysResult = false;

            foreach (var checkRay in checkRays)
            {
                if (checkRay)
                {
                    checkRaysResult = true;
                    break;
                }
            }

            if (checkRaysResult)
            {
                foreach (var hitRay in hitRays)
                {
                    pushV += hitRay.normal * settings.pushSpeed;
                }
            }
            pushV.y = 0f;
            desiredV += pushV;                
        }
        
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
                statusPlayer.Die(DeadType.FallClash);
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
                    if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
                    {
                        statusPlayer.Die(DeadType.FallClash);
                    }
                    else
                    {
                        damagePlayer.Damage(gravityDamageInfo);
                    }
                }
                else if (gravityVelocity > settings.damageValue)
                {
                    //落下ダメージ
                    gravityDamageInfo.DamageType = DamageType.Falling;
                    gravityDamageInfo.AttackPower = 20;
                    damagePlayer.Damage(gravityDamageInfo);
                }
            }
            gravityVelocity = 0f;
        }
        else
        {
            isGrounded = false;
            gravityVelocity += settings.gravityAcceleration * Time.deltaTime;
            gravityVelocity = Mathf.Clamp(gravityVelocity, 0f, settings.gravityVelocityMax);

            if (transform.position.y < -25f) statusPlayer.Die(DeadType.DropOut);
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
            
            if (IsGrounded &&
                gravityDamageInfo.ShockPower > 200 &&
                statusPlayer.PlayerStatus == PlayerStatus.FallDown)
            {
                statusPlayer.PlayerStatus = PlayerStatus.Recover;
            }
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
}
