using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class CameraManagerWin : MonoBehaviour
{
    [SerializeField] float cameraHeight = 1f;
    [SerializeField] float lookHeight = 1.5f;
    [SerializeField] float distance = 2f;
    [SerializeField] float pitchCameraSpeed = 15f;
    [SerializeField] float pitchCameraSpeedBase = -50f;
    [SerializeField] float yawCameraSpeed = 15f;
    [SerializeField] float yawCameraSpeedBase = -50f;
    [SerializeField] Transform player;
    [SerializeField] float pitch;
    [SerializeField] float yaw;
    [SerializeField] float pitchMin = -30f;
    [SerializeField] float pitchMax = 45f;

    CancellationToken destroyToken;

    void OnEnable()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
        PitchAsync(destroyToken).Forget(ex => Debug.LogError($"PitchAsyncで例外エラー: {ex}"));
        YawAsync(destroyToken).Forget(ex => Debug.LogError($"YawAsyncで例外エラー: {ex}"));
    }
    void LateUpdate()
    {
        Vector3 playerPos = player.position;
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rot * new Vector3(0f, 0f, -distance);
        Vector3 pos = playerPos + offset + Vector3.up * cameraHeight;
        Vector3 originPos = playerPos + Vector3.up * lookHeight;
        Quaternion lookRot = Quaternion.LookRotation(
            originPos - pos).normalized;
        Vector3 offsetDir = lookRot * Vector3.forward;
        transform.position = pos;
        transform.rotation = lookRot;
    }

    async UniTask PitchAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                int rnd = UnityEngine.Random.Range(0, 10);
                float duration = 5f + rnd * 1f;
                float timer = 0f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    rnd = UnityEngine.Random.Range(0, 3);
                    var pitchCameraSpeedTarget =
                        -pitchCameraSpeedBase + rnd * pitchCameraSpeedBase;
                    pitchCameraSpeed =
                        Mathf.Lerp(
                            pitchCameraSpeed,
                            pitchCameraSpeedTarget,
                            Time.deltaTime);
                    pitch += pitchCameraSpeed * Time.deltaTime;
                    pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
                    await UniTask.Yield(cancellationToken: ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("PitchAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    async UniTask YawAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                int rnd = UnityEngine.Random.Range(0, 10);
                float duration = 5f + rnd * 1f;
                float timer = 0f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    rnd = UnityEngine.Random.Range(0, 3);
                    var yawCameraSpeedTarget = -yawCameraSpeedBase + rnd * yawCameraSpeedBase;
                    yawCameraSpeed =
                        Mathf.Lerp(
                            yawCameraSpeed,
                            yawCameraSpeedTarget,
                            Time.deltaTime);
                    yaw += yawCameraSpeed * Time.deltaTime;
                    await UniTask.Yield(cancellationToken: ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("yawAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
