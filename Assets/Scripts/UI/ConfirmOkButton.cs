using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConfirmOkButton : MonoBehaviour
{
    [SerializeField] PausePanel pausePanel;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        if (pausePanel != null) pausePanel.QuitGame();
        else QuitGame();
    }
    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }
}
