using UnityEngine;
using UnityEngine.UI;

public class ResultButton : MonoBehaviour
{
    [SerializeField] StartMenuPanel startMenuPanel;
    [SerializeField] GameObject resultPanel;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        ((Panel)startMenuPanel).SaveSelected();
        startMenuPanel.gameObject.SetActive(false);
        resultPanel.SetActive(true);
    }
}
