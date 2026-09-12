using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenModeItem : MonoBehaviour, IPointerClickHandler, IMoveHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueTmp;
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] ResolutionItem resolutionItem;
    [SerializeField] GameObject cancelButton;

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos);
        if (localPos.x < 0) LeftButton();
        else RightButton();
    }
    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Left) LeftButton();
        if (eventData.moveDir == MoveDirection.Right) RightButton();
        if (eventData.moveDir == MoveDirection.Up)
        {
            EventSystem.current.SetSelectedGameObject(cancelButton);
        }
    }

    void LeftButton()
    {
        GameSystem.ConfigTmp.IsFullScreen = !GameSystem.ConfigTmp.IsFullScreen;
        ShowScreenMode();
        resolutionItem.Interactable = !GameSystem.ConfigTmp.IsFullScreen;
    }
    void RightButton()
    {
        GameSystem.ConfigTmp.IsFullScreen = !GameSystem.ConfigTmp.IsFullScreen;
        ShowScreenMode();
        resolutionItem.Interactable = !GameSystem.ConfigTmp.IsFullScreen;
    }

    public void ShowScreenMode()
    {
        var s = GameSystem.ConfigTmp.IsFullScreen ? "FullScreen" : "Window";
        valueTmp.text = s;
    }
}
