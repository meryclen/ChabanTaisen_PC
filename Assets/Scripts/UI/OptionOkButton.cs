using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OptionOkButton : MonoBehaviour, IMoveHandler
{
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] GameObject screenSettingsPanel;
    [SerializeField] GameObject soundSettingsPanel;
    [SerializeField] GameObject difficultySettingsPanel;
    [SerializeField] GameObject optionCancelButton;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        GameSystem.Config.CopyFrom(GameSystem.ConfigTmp);
        optionMenuPanel.gameObject.SetActive(true);
        if (screenSettingsPanel != null) screenSettingsPanel.SetActive(false);
        if (soundSettingsPanel != null) soundSettingsPanel.SetActive(false);
        if (difficultySettingsPanel != null)
        {
            difficultySettingsPanel.SetActive(false);
        }
        optionMenuPanel.IsUpdated = true;
    }

    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(optionCancelButton);
        }
    }
}
