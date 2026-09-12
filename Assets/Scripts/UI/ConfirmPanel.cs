using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] GameObject cancelButton;
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(cancelButton);
    }
    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}
