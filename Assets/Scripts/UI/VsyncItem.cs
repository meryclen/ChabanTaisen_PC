using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class VsyncItem : MonoBehaviour, IPointerClickHandler, IMoveHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueTmp;
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] FrameRateItem frameRateItem;

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
    }

    void LeftButton()
    {
        GameSystem.ConfigTmp.IsVsync = !GameSystem.ConfigTmp.IsVsync;
        ShowVsync();
        frameRateItem.Interactable = !GameSystem.ConfigTmp.IsVsync;
    }
    void RightButton()
    {
        GameSystem.ConfigTmp.IsVsync = !GameSystem.ConfigTmp.IsVsync;
        ShowVsync();
        frameRateItem.Interactable = !GameSystem.ConfigTmp.IsVsync;
    }

    public void ShowVsync()
    {
        var s = GameSystem.ConfigTmp.IsVsync ? "On" : "Off";
        valueTmp.text = s;
    }
}
