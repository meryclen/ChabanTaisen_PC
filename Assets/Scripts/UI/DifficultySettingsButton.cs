using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DifficultySettingsButton : MonoBehaviour, IMoveHandler
{
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] GameObject difficultySettingsPanel;
    [SerializeField] GameObject bottomMenuButton;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        ((Panel)optionMenuPanel).SaveSelected();
        optionMenuPanel.gameObject.SetActive(false);
        difficultySettingsPanel.SetActive(true);
    }

    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Up)
        {
            EventSystem.current.SetSelectedGameObject(bottomMenuButton);
        }
    }
}
