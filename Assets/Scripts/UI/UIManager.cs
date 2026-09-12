using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] InputActionReference cancel;
    ICancelable curPanel;

    void OnEnable()
    {
        cancel.action.performed += OnCancel;
    }
    void OnDisable()
    {
        cancel.action.performed -= OnCancel;
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (curPanel != null)
        {
            Log.D($"curPanel: {curPanel}");
            curPanel.Cancel();
        }
        else Log.D($"curPanel: {curPanel}");
    }

    public void SetCurPanel(ICancelable curPanel)
    {
        this.curPanel = curPanel;
    }
}
