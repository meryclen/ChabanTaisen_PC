using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class ShapeTitle : Shape
{
    [SerializeField] Confuse blw_def_conf;
    [SerializeField] Confuse mth_def_sap;

    
    protected override void OnEnable()
    {
        base.OnEnable();
        ShapeConfuseAsync(linkedToken).Forget();
        ShapeBlinkAsync(linkedToken).Forget();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    protected override void OnDestroy()
    {
        cts?.Dispose();
        linkedCts?.Dispose();
    }
    
    async UniTask ShapeConfuseAsync(CancellationToken ct)
    {
        try
        {
            blw_def_conf.value = 50f;
            mth_def_sap.value = 50f;

            while (true)
            {
                await ConfuseAsync(ct);
                int rnd = UnityEngine.Random.Range(0, 10);
                await UniTask.Delay(500 + rnd * 100, cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("ShapeConfuseAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    async UniTask ShapeBlinkAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                int rnd = UnityEngine.Random.Range(0, 10);
                await UniTask.Delay(5000 + rnd * 1000, cancellationToken: ct);                
                await BlinkAsync(0f, 0f, 0f, 60f, 35f, 40f, ct);
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("ShapeBlinkAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    async UniTask ConfuseAsync(CancellationToken ct)
    {
        int rnd = UnityEngine.Random.Range(0, 5);
        float duration = 0.1f + rnd * 0.1f;
        blw_def_conf.SetSpeed();
        mth_def_sap.SetSpeed();
        while (duration > 0f)
        {
            duration -= Time.deltaTime;
            blw_def_conf.SetValue();
            mth_def_sap.SetValue();
            smr_blw_def.SetBlendShapeWeight(blw_def_conf_index, blw_def_conf.value);
            smr_mth_def.SetBlendShapeWeight(mth_def_sap_index, mth_def_sap.value);
            await UniTask.Yield(cancellationToken: ct);
        }
    }
    async UniTask BlinkAsync(
        float init_el_def_def_c,
        float init_eye_def_smile1,
        float init_eye_def_def_c,
        float target_el_def_def_c,
        float target_eye_def_smile1,
        float target_eye_def_def_c,
        CancellationToken ct)
    {
        float el_def_def_c;
        float eye_def_smile1;
        float eye_def_def_c;        

        const float duration = 0.1f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float weight = Mathf.Lerp(0f, 1f, t);

            el_def_def_c = init_el_def_def_c +
                (target_el_def_def_c - init_el_def_def_c) * weight;
            eye_def_smile1 = init_eye_def_smile1 +
                (target_eye_def_smile1 - init_eye_def_smile1) * weight;
            eye_def_def_c = init_eye_def_def_c +
                (target_eye_def_def_c - init_eye_def_def_c) * weight;

            smr_el_def.SetBlendShapeWeight(el_def_def_c_index, el_def_def_c);
            smr_eye_def.SetBlendShapeWeight(eye_def_smile1_index, eye_def_smile1);
            smr_eye_def.SetBlendShapeWeight(eye_def_def_c_index, eye_def_def_c);

            await UniTask.Yield(cancellationToken: ct);
        }
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float weight = Mathf.Lerp(0f, 1f, t);

            el_def_def_c = target_el_def_def_c +
                (init_el_def_def_c - target_el_def_def_c) * weight;
            eye_def_smile1 = target_eye_def_smile1 +
                (init_eye_def_smile1 - target_eye_def_smile1) * weight;
            eye_def_def_c = target_eye_def_def_c +
                (init_eye_def_def_c - target_eye_def_def_c) * weight;

            smr_el_def.SetBlendShapeWeight(el_def_def_c_index, el_def_def_c);
            smr_eye_def.SetBlendShapeWeight(eye_def_smile1_index, eye_def_smile1);
            smr_eye_def.SetBlendShapeWeight(eye_def_def_c_index, eye_def_def_c);

            await UniTask.Yield(cancellationToken: ct);
        }
    }
}

[System.Serializable]
public class Confuse
{
    public float value;
    public float speed;
    public float min;
    public float max;
    public void SetValue()
    {
        value += speed;
        value = Mathf.Clamp(value, min, max);
    }
    public void SetSpeed()
    {
        int rnd = UnityEngine.Random.Range(0, 2);
        speed = rnd == 0 ? 0.5f : -0.5f;
    }
}
