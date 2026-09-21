using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenuButton : MonoBehaviour, IMoveHandler
{
    [SerializeField] GameObject startMenuPanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject optionMenuPanelGO;
    [SerializeField] GameObject topButton;
    OptionMenuPanel optionMenuPanel;

    void Awake()
    {
        if (optionMenuPanelGO != null) optionMenuPanel =
                optionMenuPanelGO.GetComponent<OptionMenuPanel>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {        
        startMenuPanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);
        if (optionMenuPanelGO != null)
        {
            if (optionMenuPanel.IsUpdated) optionMenuPanel.WriteJson();
            optionMenuPanel.gameObject.SetActive(false);
        }
    }

    public void OnMove(AxisEventData eventData)
    {
        if (optionMenuPanelGO != null && eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(topButton);
        }
    }
}
