using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SoundSettingsButton : MonoBehaviour
{
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] GameObject soundSettingsPanel;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        ((Panel)optionMenuPanel).SaveSelected();
        optionMenuPanel.gameObject.SetActive(false);
        soundSettingsPanel.SetActive(true);
    }
}
