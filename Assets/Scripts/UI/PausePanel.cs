using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

public class PausePanel : MonoBehaviour
{
    [SerializeField] StatusPlayer statusPlayer;
    [SerializeField] GameObject resumeButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject titleButton;
    [SerializeField] GameObject quitGameButton;
    [SerializeField] GameManager gameManager;

    [SerializeField] InputActionReference cancel;
    CancellationToken destroyToken;

    void Awake()
    {
        destroyToken = this.GetCancellationTokenOnDestroy();
    }
    void OnEnable()
    {
        statusPlayer.OnPause += Refresh;
        statusPlayer.OnPauseCancel += PauseCancel;
        cancel.action.performed += OnCancel;
    }
    void OnDisable()
    {
        statusPlayer.OnPause -= Refresh;
        statusPlayer.OnPauseCancel -= PauseCancel;
        cancel.action.performed -= OnCancel;
    }

    public void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Refresh()
    {
        resumeButton.SetActive(true);
        restartButton.SetActive(true);
        titleButton.SetActive(true);
        quitGameButton.SetActive(true);
    }
    public void PauseCancel()
    {
        resumeButton.SetActive(false);
        restartButton.SetActive(false);
        titleButton.SetActive(false);
        quitGameButton.SetActive(false);
    }
    public void QuitGame()
    {
        gameManager.QuitGame();
    }

    public void ResumeGame()
    {
        ResumeGameAsync(destroyToken).Forget(
            ex => Debug.LogError("ResumeGameAsyncÇ≈ó·äOÉGÉâÅ[: {ex}"));
    }

    async UniTask ResumeGameAsync(CancellationToken ct)
    {
        await UniTask.Yield(cancellationToken: ct);

        gameManager.InputSystemManager.SetState(GameState.GamePlay);

        PauseCancel();
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        if (gameManager.InputSystemManager.GetState() == GameState.Dead) return;
        if (context.performed) ResumeGame();
    }
}
