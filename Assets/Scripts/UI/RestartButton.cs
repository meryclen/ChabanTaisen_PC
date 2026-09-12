using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartButton : MonoBehaviour
{
    void Awake()
    {
        Log.D("RestartButton Awake");
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
