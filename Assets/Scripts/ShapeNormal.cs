using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class ShapeNormal : Shape
{
    [SerializeField] StatusPlayer statusPlayer;

    float blw_def_conf_weight;
    float el_def_def_c_weight;
    float eye_def_smile1_weight;
    float eye_def_def_c_weight;
    float mth_def_smile1_weight;
    float mth_def_smile1_targetWeight;

    void Awake()
    {
        int rnd = UnityEngine.Random.Range(0, 5);
        float mth_def_smile1_targetWeight = 50f + rnd * 5f;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        statusPlayer.OnDamage += Damage;
        statusPlayer.OnDropOut += DropOut;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        statusPlayer.OnDamage -= Damage;
        statusPlayer.OnDropOut -= DropOut;
    }

    void Damage(PlayerStatus playerStatus, int shockPower)
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();
        linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                destroyToken, cts.Token);
        linkedToken = linkedCts.Token;

        DamageAsync(linkedToken, playerStatus, shockPower).Forget(
            ex => Debug.LogError($"DamageAsyncで例外エラー: {ex}"));
    }
    async UniTask DamageAsync(CancellationToken ct, PlayerStatus playerStatus, int shockPower)
    {
        try
        {
            smr_blw_def.SetBlendShapeWeight(blw_def_conf_index, 100f);
            smr_el_def.SetBlendShapeWeight(el_def_def_c_index, 45f);
            smr_eye_def.SetBlendShapeWeight(eye_def_smile1_index, 50f);
            smr_eye_def.SetBlendShapeWeight(eye_def_def_c_index, 35f);
            int rnd = UnityEngine.Random.Range(0, 5);
            float weight = 50f + rnd * 5f;
            smr_mth_def.SetBlendShapeWeight(mth_def_smile1_index, weight);

            float duration = 0f;
            if (playerStatus == PlayerStatus.Dead) return;

            if (shockPower >= 200) duration = 2f;
            else if (playerStatus == PlayerStatus.Recover)　duration = 1f;
            else if (shockPower >= 10) duration = 0.5f;
            else duration = 0.5f;

            await UniTask.Delay((int)(duration * 1000), cancellationToken: ct);

            duration = 0.5f;
            while (duration > 0f)
            {
                duration -= Time.deltaTime;
                smr_blw_def.SetBlendShapeWeight(blw_def_conf_index, 100f * duration);
                smr_el_def.SetBlendShapeWeight(el_def_def_c_index, 45f * duration);
                smr_eye_def.SetBlendShapeWeight(eye_def_smile1_index, 50f * duration);
                smr_eye_def.SetBlendShapeWeight(eye_def_def_c_index, 35f * duration);
                smr_mth_def.SetBlendShapeWeight(mth_def_smile1_index, weight * duration);
                await UniTask.Yield(cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("DamageAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    void DropOut()
    {
        blw_def_conf_weight = Mathf.Lerp(blw_def_conf_weight, 100f, 2f * Time.deltaTime);
        el_def_def_c_weight = Mathf.Lerp(el_def_def_c_weight, 45f, 2f * Time.deltaTime);
        eye_def_smile1_weight = Mathf.Lerp(eye_def_smile1_weight, 50f, 2f * Time.deltaTime);
        eye_def_def_c_weight = Mathf.Lerp(eye_def_def_c_weight, 35f, 2f * Time.deltaTime);
        mth_def_smile1_weight =
            Mathf.Lerp(mth_def_smile1_weight, mth_def_smile1_targetWeight, 2f * Time.deltaTime);

        smr_blw_def.SetBlendShapeWeight(blw_def_conf_index, blw_def_conf_weight);
        smr_el_def.SetBlendShapeWeight(el_def_def_c_index, el_def_def_c_weight);
        smr_eye_def.SetBlendShapeWeight(eye_def_smile1_index, eye_def_smile1_weight);
        smr_eye_def.SetBlendShapeWeight(eye_def_def_c_index, eye_def_def_c_weight);
        smr_mth_def.SetBlendShapeWeight(mth_def_smile1_index, mth_def_smile1_weight);
    }    
}
