using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OptionCancelButton : MonoBehaviour, IMoveHandler
{
    [SerializeField] GameObject optionMenuPanel;
    [SerializeField] GameObject screenSettingsPanel;
    [SerializeField] GameObject soundSettingsPanel;
    [SerializeField] GameObject difficultySettingsPanel;

    [SerializeField] GameObject upItem;
    [SerializeField] GameObject downItem;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        optionMenuPanel.SetActive(true);
        if (screenSettingsPanel != null) screenSettingsPanel.SetActive(false);
        if (soundSettingsPanel != null) soundSettingsPanel.SetActive(false);
        if (difficultySettingsPanel != null) difficultySettingsPanel.SetActive(false);
    }

    public void OnMove(AxisEventData eventData)
    {        
        if (eventData.moveDir == MoveDirection.Up)
        {
            EventSystem.current.SetSelectedGameObject(upItem);
        }
        if (eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(downItem);
        }
    }
}
