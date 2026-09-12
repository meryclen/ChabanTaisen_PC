using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResumeButton : MonoBehaviour, IMoveHandler
{
    [SerializeField] PausePanel pausePanel;
    [SerializeField] GameObject quitGameButton;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnEnable()
    {
        pausePanel.SetSelected();
    }
    void OnClick()
    {
        pausePanel.ResumeGame();
    }

    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Up)
        {
            EventSystem.current.SetSelectedGameObject(quitGameButton);
        }
    }
}
