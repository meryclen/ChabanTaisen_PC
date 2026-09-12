using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] MeshRenderer quadMr;
    [SerializeField] Transform quadTrans;
    [SerializeField] Texture2D texture;
    [SerializeField] MeshRenderer goalMr;

    CancellationToken destroyToken;

    Camera cam;

    void Start()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();

        var coloring = new Coloring(quadMr);
        coloring.ColorSetting(1f, 1f, 0f, 1f, 5f);
        coloring.TextureSetting(texture);

        coloring = new Coloring(goalMr);
        coloring.ColorSetting(1f, 1f, 0f, 1f, 5f);

        cam = Camera.main;

        TargetPointAsync(destroyToken).Forget(
            ex => Debug.LogError($"TargetPointAsyncで例外エラー: {ex}"));
    }

    void LateUpdate()
    {
        transform.forward = cam.transform.forward;
    }

    async UniTask TargetPointAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                int loop = 2;
                while (loop >= 0)
                {
                    loop--;
                    Vector3 tmp = quadTrans.localPosition;
                    tmp.y -= 1.5f;
                    quadTrans.localPosition = tmp;
                    await UniTask.Delay(500, cancellationToken: ct);
                }
                await UniTask.Delay(1000, cancellationToken: ct);
                quadTrans.localPosition = Vector3.zero;
            }
        }
        catch (OperationCanceledException)
        {
            Log.D("TargetPointAsyncがキャンセルされました");
        }
    }
}
