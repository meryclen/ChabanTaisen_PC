using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButton : MonoBehaviour, IMoveHandler
{
    [SerializeField] GameObject quitGameButton;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        SceneManager.LoadScene("Stage" + GameSystem.GameData.CurStage.ToString());
    }

    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Up)
        {
            EventSystem.current.SetSelectedGameObject(quitGameButton);
        }
    }
}
