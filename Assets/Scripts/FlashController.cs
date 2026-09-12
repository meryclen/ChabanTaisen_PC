using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class FlashController : MonoBehaviour
{
    Renderer[] renderers;
    List<Coloring> coloringList = new();

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();        

        foreach (var renderer in renderers)
        {
            if (renderer.CompareTag("IgnoreFlash")) continue;
            var coloring = new Coloring(renderer);
            coloringList.Add(coloring);
        }
    }
    void OnEnable()
    {
        foreach (var coloring in coloringList)
        {
            coloring.ColorSetting(1f, 1f, 1f, 1f, 0f);
        }
    }

    public async UniTask FlashAsync(CancellationTokenSource linkedCts)
    {
        try
        {
            foreach (var coloring in coloringList)
            {
                coloring.ColorSetting(1f, 1f, 1f, 1f, 20f);
            }

            await UniTask.Delay(200, cancellationToken: linkedCts.Token);

            foreach (var coloring in coloringList)
            {
                coloring.ColorSetting(1f, 1f, 1f, 1f, 0f);
            }
        }
        catch (OperationCanceledException)
        {
            Log.D($"FlashAsync‚ªƒLƒƒƒ“ƒZƒ‹‚³‚ê‚Ü‚µ‚½");
        }        
        finally
        {
            linkedCts.Dispose();
        }        
    }
}
