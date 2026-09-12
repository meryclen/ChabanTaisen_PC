using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConfirmCancelButton : MonoBehaviour
{
    [SerializeField] ConfirmPanel confirmPanel;
    [SerializeField] InputActionReference cancel;
    [SerializeField] TitleManager titleManager;
    [SerializeField] PausePanel pausePanel;
    [SerializeField] StartMenuPanel startMenuPanel;
    [SerializeField] GameObject resumeButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject titleButton;
    [SerializeField] GameObject quitGameButton;

    void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        confirmPanel = transform.parent.GetComponent<ConfirmPanel>();
        cancel.action.performed += OnCancel;
    }
    void OnDisable()
    {
        cancel.action.performed -= OnCancel;
    }

    void OnClick()
    {
        Cancel();
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Cancel();
        }
    }

    void Cancel()
    {
        confirmPanel.Cancel();
        if (titleManager != null)
        {
            startMenuPanel.gameObject.SetActive(true);
        }
        if (pausePanel != null)
        {
            resumeButton.SetActive(true);
            restartButton.SetActive(true);
            titleButton.SetActive(true);
            quitGameButton.SetActive(true);
            pausePanel.SetSelected();
        }
    }
}
