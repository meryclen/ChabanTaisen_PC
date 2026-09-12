using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StatusPlayer : StatusController
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject pistol2;

    CancellationTokenSource cts;
    CancellationToken destroyToken;
    CancellationTokenSource linkedCts;
    CancellationToken linkedToken;

    [SerializeField] int maxHp = 12;
    [SerializeField] int hp = 12;
    public override int MaxHp { get => maxHp; set => maxHp = value; }
    public override int Hp { get => hp; set => hp = value; }

    public override bool IsHpZero => hp <= 0;
    bool deathProcessed;
    [SerializeField] PlayerStatus playerStatus = PlayerStatus.Fine;
    public PlayerStatus PlayerStatus { get => playerStatus; set => playerStatus = value; }

    [SerializeField] Anim anim;

    public event Action OnHpChanged;
    public override void RaiseHpChanged() { OnHpChanged?.Invoke(); }
    public event Action OnGameOver;
    public event Action OnGameOverDropOut;
    public event Action OnRestart;
    public event Action OnPause;
    public override void RaisePause() { OnPause?.Invoke(); }
    public event Action OnPauseCancel;
    public event Action OnTimeOver;
    public event Action OnWin;
    public event Action<PlayerStatus, int> OnDamage;
    public void RaiseDamage(PlayerStatus playerStatus, int shockPower)
    {
        OnDamage?.Invoke(playerStatus, shockPower);
    }
    public event Action OnDropOut;
    public void RaiseDropOut() { OnDropOut?.Invoke(); }


    void OnEnable()
    {
        cts = new CancellationTokenSource();
        destroyToken = this.GetCancellationTokenOnDestroy();
        linkedCts = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, cts.Token);
        linkedToken = linkedCts.Token;

        deathProcessed = false;
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

    public override void Die(DeadType deadType)
    {
        if (deathProcessed) return;
        deathProcessed = true;
        anim.DeadType = deadType;
        if (deadType == DeadType.DropOut) OnGameOverDropOut?.Invoke();
        if (deadType == DeadType.TimeOver) OnTimeOver?.Invoke();
        else
        {
            hp = 0;
            playerStatus = PlayerStatus.Dead;
            pistol2.SetActive(false);
        }
        
        GameOverAsync(linkedToken).Forget(ex => Debug.LogError($"GameOverAsyncÇ≈ó·äOÉGÉâÅ[: {ex}"));
    }

    async UniTask GameOverAsync(CancellationToken ct)
    {
        try
        {
            gameManager.InputSystemManager.SetState(GameState.Dead);
            OnPauseCancel?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: ct);
            OnGameOver?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: ct);
            OnRestart?.Invoke();
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void Win()
    {
        OnWin?.Invoke();
        gameManager.InputSystemManager.SetState(GameState.SceneChange);
        SceneManager.LoadScene("StageClear");
    }
}
