using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameClearPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] TextMeshProUGUI clearTimeTmp;
    [SerializeField] TextMeshProUGUI updateBestTime;
    [SerializeField] GameObject titleButton;    

    CancellationToken destroyToken;

    float idleTimer;
    const float idleTimerLimit = 30f;

    void Start()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();

        tmp.text = GameSystem.GameData.CurStage.ToString() + "Stage Clear!";

        int timer_int = GameSystem.GameData.CurStageClearTime;
        int timer_int_m = timer_int / 60;
        int timer_int_s = timer_int % 60;

        string timer_s_m;
        if (timer_int_m < 10) timer_s_m = timer_int_m.ToString();
        else timer_s_m = timer_int_m.ToString();

        string timer_s_s;
        if (timer_int_s < 10) timer_s_s = '0' + timer_int_s.ToString();
        else timer_s_s = timer_int_s.ToString();

        clearTimeTmp.text = "ClearTime:  " + timer_s_m + ':' + timer_s_s;
        updateBestTime.text = null;

        EventSystem.current.SetSelectedGameObject(titleButton);

        if (timer_int <
            GameSystem.GameData.DifficultyDatas[(int)GameSystem.Config.Difficulty].
            StageDatas[GameSystem.GameData.CurStage - 1].BestStageClearTime)
        {
            updateBestTime.text = "ベストタイムを更新しました！";
            GameSystem.GameData.DifficultyDatas[(int)GameSystem.Config.Difficulty].
                StageDatas[GameSystem.GameData.CurStage - 1].BestStageClearTime = timer_int;
            GameSystem.SaveUtility.Save<GameData>(
                Path.Combine(Application.persistentDataPath, "save.dat"),
                GameSystem.GameData);
        }

        IdleTimerAsync(destroyToken).Forget(ex => Debug.LogError("IdleTimerAsyncで例外エラー"));
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed) ResetIdleTimer();
    }
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed) ResetIdleTimer();
    }
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed) ResetIdleTimer();
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed) ResetIdleTimer();
    }
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed) ResetIdleTimer();
    }

    void ResetIdleTimer()
    {
        Log.D("Raise ResetIdeleTimer()");
        idleTimer = 0f;
    }

    async UniTask IdleTimerAsync(CancellationToken ct)
    {
        try
        {
            while (true)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer > idleTimerLimit)
                {
                    ReturnTitle();
                    throw new OperationCanceledException();
                }
                await UniTask.Yield(cancellationToken: ct);
            }
        }
        catch (OperationCanceledException ex)
        {
            Log.D($"IdleTimerAsyncがキャンセルされました: {ex}");
        }
    }
    void ReturnTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
