using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public abstract class FingerCtrl : MonoBehaviour
{
    [SerializeField] protected float defaultPitch = 15f;
    [SerializeField] protected Finger[] fingers;

    CancellationTokenSource cts;
    CancellationToken destroyToken;
    CancellationTokenSource linkedCts;
    CancellationToken linkedToken;

    [SerializeField] protected float speedBase = 0.1f;

    [SerializeField] protected bool isGrip;
    [SerializeField] float speedBaseRate = 1f;

    [SerializeField] protected Anim anim;


    void OnEnable()
    {
        cts = new CancellationTokenSource();
        destroyToken = this.GetCancellationTokenOnDestroy();
        linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cts.Token);
        linkedToken = linkedCts.Token;

        GripAsync(linkedToken).Forget(ex => Debug.LogError($"GripAsyncで例外エラー: {ex}"));
    }
    void Start()
    {
        for (int i=0; i<fingers.Length; i++)
        {
            fingers[i].offsetRot = fingers[i].fingerConstraints[0].bone.localRotation;
        }
    }
    void OnDisable()
    {
        cts?.Cancel();
        linkedCts?.Dispose();
        linkedCts = null;
        cts?.Dispose();
        cts = null;
    }
    void OnDestroy()
    {
        cts?.Dispose();
        linkedCts?.Dispose();
    }

    protected abstract void LateUpdate();

    protected abstract void Grip();

    protected void DefaultGrip()
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            for (int j = 0; j < fingers[i].fingerConstraints.Length; j++)
            {
                fingers[i].fingerConstraints[j].pitch =
                    defaultPitch * fingers[i].fingerConstraints[j].rate;
                fingers[i].fingerConstraints[j].target.localRotation =
                    Quaternion.AngleAxis(fingers[i].fingerConstraints[j].pitch, Vector3.right);
            }
        }
    }

    async UniTask GripAsync(CancellationToken ct)
    {
        try
        {
            await UniTask.Delay(1000, cancellationToken: ct);
            while (true)
            {
                int timeRnd = UnityEngine.Random.Range(0, 10);
                await UniTask.Delay(1000 + timeRnd * 1000, cancellationToken: ct);
                int isGripRnd = UnityEngine.Random.Range(0, 5);
                isGrip = (isGripRnd != 0 && isGripRnd != 1) ? true : false;
                if (isGrip)
                {
                    int speedRnd = UnityEngine.Random.Range(0, 2);
                    int speedRateRnd = UnityEngine.Random.Range(0, 2);
                    speedBase =
                        (speedRnd == 0 ? -0.1f * speedBaseRate : 0.1f * speedBaseRate) *
                        (speedRateRnd == 0 ? 0.5f : 1f);
                    timeRnd = UnityEngine.Random.Range(0, 5);
                    await UniTask.Delay(100 + timeRnd * 100, cancellationToken: ct);
                    isGrip = false;
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("GripAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}

[System.Serializable]
public class Finger
{
    public FingerConstraint[] fingerConstraints;
    public Transform targetBase;
    public Quaternion offsetRot;
}

[System.Serializable]
public class FingerConstraint
{
    public MultiRotationConstraint constraint;
    public Transform bone;
    public Transform target;
    public float rate = 1f;
    public float min;
    public float max;
    public float pitch;
}
