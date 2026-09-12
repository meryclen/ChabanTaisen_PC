using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public abstract class TopPanel : MonoBehaviour
{
    CancellationToken destroyToken;
    [SerializeField] protected TextMeshProUGUI[] storyTexts;
    protected virtual void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
    }
    void OnEnable()
    {
        TypeTextAsync(destroyToken).Forget();
    }
    async UniTask TypeTextAsync(CancellationToken ct)
    {
        try
        {
            foreach (var t in storyTexts)
            {
                t.ForceMeshUpdate();
                t.maxVisibleCharacters = 0;
            }
            for (int i = 0; i < storyTexts.Length; i++)
            {
                int count = storyTexts[i].textInfo.characterCount;
                for (int j = 0; j < count; j++)
                {
                    storyTexts[i].maxVisibleCharacters = j + 1;
                    await UniTask.Delay(100, cancellationToken: ct);
                }
                await UniTask.Delay(1000, cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
