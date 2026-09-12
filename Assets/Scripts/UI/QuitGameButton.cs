using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour, IMoveHandler
{
    [SerializeField] GameObject confirmPanel;
    [SerializeField] StartMenuPanel startMenuPanel;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject resumeButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject titleButton;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        confirmPanel.SetActive(true);
        if (startMenuPanel != null)
        {
            ((Panel)startMenuPanel).SaveSelected(gameObject);
            startMenuPanel.gameObject.SetActive(false);
        }

        if (resumeButton != null)
        {
            resumeButton.SetActive(false);
            restartButton.SetActive(false);
            titleButton.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void OnMove(AxisEventData eventData)
    {
        if (resumeButton != null && eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
        if (resumeButton == null && eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(startButton);
        }
    }
}
