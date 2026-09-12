using UnityEngine;
using UnityEngine.EventSystems;

public class Panel : MonoBehaviour
{
    [SerializeField] protected GameObject topButton;
    protected GameObject lastSelected;

    protected virtual void Awake()
    {

    }
    protected virtual void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(
            lastSelected != null ? lastSelected : topButton);
    }
    public virtual void SaveSelected()
    {
        lastSelected = EventSystem.current.currentSelectedGameObject;
    }
    public virtual void SaveSelected(GameObject go)
    {
        lastSelected = go;
    }
}
