using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] Enemy enemy0;
    [SerializeField] Enemy enemy1;
    [SerializeField] Enemy enemy2;
    [SerializeField] PoolManager poolManager;

    CancellationTokenSource cts;
    CancellationToken destroyToken;
    CancellationTokenSource linkedCts;
    CancellationToken linkedToken;

    [SerializeField] Stage stage;
    [SerializeField] PlayerInput playerInput;
    InputSystemManager inputSystemManager;
    public InputSystemManager InputSystemManager => inputSystemManager;


    void Awake()
    {
        stage.Initialize(enemy0, enemy1, enemy2, poolManager);

        inputSystemManager = new InputSystemManager(playerInput);
        inputSystemManager.SetState(GameState.GamePlay);
    }
    void Start()
    {
        cts = new CancellationTokenSource();
        destroyToken = this.GetCancellationTokenOnDestroy();
        linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cts.Token);
        linkedToken = linkedCts.Token;

        GameAsync(linkedToken).Forget(ex => Debug.LogError($"GameAsyncで例外エラー: {ex}"));        
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

    async UniTask GameAsync(CancellationToken ct)
    {
        try
        {
            ct.ThrowIfCancellationRequested();
            await stage.DefaultSpawnEnemy0(linkedToken);
            await stage.StageAsync(linkedToken);
        }
        catch (OperationCanceledException)
        {
            Log.D("GameAsyncがキャンセルされました");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
