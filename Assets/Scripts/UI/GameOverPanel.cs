using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] StatusPlayer statusPlayer;
    [SerializeField] GameObject gameOverText;
    [SerializeField] GameObject timeOverText;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject titleButton;

    [SerializeField] InputActionReference navigate;
    [SerializeField] InputActionReference submit;
    [SerializeField] InputActionReference cancel;
    [SerializeField] InputActionReference click;
    [SerializeField] InputActionReference rightClick;

    CancellationToken destroyToken;

    float idleTimer;
    const float idleTimerLimit = 20f;


    void OnEnable()
    {
        statusPlayer.OnGameOver += RefreshGameOver;
        statusPlayer.OnRestart += RefreshRestart;
        statusPlayer.OnTimeOver += RefreshTimeOver;
        navigate.action.performed += OnNavigate;
        submit.action.performed += OnSubmit;
        cancel.action.performed += OnCancel;
        click.action.performed += OnClick;
        rightClick.action.performed += OnRightClick;
        destroyToken = this.GetCancellationTokenOnDestroy();
    }
    void OnDisable()
    {
        statusPlayer.OnGameOver -= RefreshGameOver;
        statusPlayer.OnRestart -= RefreshRestart;
        statusPlayer.OnTimeOver -= RefreshTimeOver;
        navigate.action.performed -= OnNavigate;
        submit.action.performed -= OnSubmit;
        cancel.action.performed -= OnCancel;
        click.action.performed -= OnClick;
        rightClick.action.performed -= OnRightClick;
    }
    public void RefreshGameOver()
    {
        gameOverText.SetActive(true);
        IdleTimerAsync(destroyToken).Forget(ex => Debug.LogError("IdleTimerAsyncで例外エラー"));
    }
    public void RefreshRestart()
    {
        restartButton.SetActive(true);
        titleButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartButton);
    }
    public void RefreshTimeOver()
    {
        timeOverText.SetActive(true);
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
