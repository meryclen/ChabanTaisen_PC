using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenSettingsButton : MonoBehaviour
{
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] GameObject screenSettingsPanel;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        ((Panel)optionMenuPanel).SaveSelected();
        optionMenuPanel.gameObject.SetActive(false);
        screenSettingsPanel.SetActive(true);
    }
}
