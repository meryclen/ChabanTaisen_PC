using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class ShapeWin : Shape
{
    [SerializeField] Smile blw_def_smile2;
    [SerializeField] Smile el_def_smile1;
    [SerializeField] Smile eye_def_smile1;
    [SerializeField] Smile eye_def_def_c;
    [SerializeField] Smile mth_def_a;

    
    protected override void OnEnable()
    {
        base.OnEnable();
        ShapeAsync(linkedToken).Forget(ex => Debug.LogError("ShapeAsyncで例外エラー: {ex}"));
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
    
    async UniTask ShapeAsync(CancellationToken ct)
    {
        try
        {
            blw_def_smile2.value = 100f;
            el_def_smile1.value = 100f;
            eye_def_smile1.value = 30f;
            eye_def_def_c.value = 30f;
            mth_def_a.value = 20f;

            while (true)
            {
                await SmileAsync(ct);
                int rnd = UnityEngine.Random.Range(0, 10);
                await UniTask.Delay(500 + rnd * 100, cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("ShapeAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    async UniTask SmileAsync(CancellationToken ct)
    {
        int rnd = UnityEngine.Random.Range(0, 5);
        float duration = 0.1f + rnd * 0.1f;
        blw_def_smile2.SetSpeed();
        el_def_smile1.SetSpeed();
        eye_def_smile1.SetSpeed();
        eye_def_def_c.speed = el_def_smile1.speed;
        mth_def_a.SetSpeed();
        while (duration > 0f)
        {
            duration -= Time.deltaTime;                
            blw_def_smile2.SetValue();
            el_def_smile1.SetValue();
            eye_def_smile1.SetValue();
            eye_def_def_c.SetValue();
            mth_def_a.SetValue();
            smr_blw_def.SetBlendShapeWeight(blw_def_smile2_index, blw_def_smile2.value);
            smr_el_def.SetBlendShapeWeight(el_def_smile1_index, el_def_smile1.value);
            smr_eye_def.SetBlendShapeWeight(eye_def_smile1_index, eye_def_smile1.value);
            smr_eye_def.SetBlendShapeWeight(eye_def_def_c_index, eye_def_def_c.value);
            smr_mth_def.SetBlendShapeWeight(mth_def_a_index, mth_def_a.value);
            await UniTask.Yield(cancellationToken: ct);
        }
    }
}

[System.Serializable]
public class Smile
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
