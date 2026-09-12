using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OptionButton : MonoBehaviour
{
    [SerializeField] StartMenuPanel startMenuPanel;
    [SerializeField] GameObject optionMenuPanelGO;
    OptionMenuPanel optionMenuPanel;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        optionMenuPanel = optionMenuPanelGO.GetComponent<OptionMenuPanel>();
    }
    void OnClick()
    {
        ((Panel)startMenuPanel).SaveSelected();
        startMenuPanel.gameObject.SetActive(false);
        optionMenuPanel.gameObject.SetActive(true);
        optionMenuPanel.IsUpdated = false;
    }
}
