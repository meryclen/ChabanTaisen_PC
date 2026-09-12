using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnOffItem : MonoBehaviour, IPointerClickHandler, IMoveHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueTmp;
    [SerializeField] SoundIndex soundsIndex;

    [SerializeField] GameObject upItem;
    [SerializeField] GameObject downItem;

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
            EventSystem.current.SetSelectedGameObject(upItem);
        }
        if (eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(downItem);
        }
    }

    void LeftButton()
    {
        GameSystem.ConfigTmp.Sounds[(int)soundsIndex] =
            !GameSystem.ConfigTmp.Sounds[(int)soundsIndex];
        ShowOnOff();
    }
    void RightButton()
    {
        GameSystem.ConfigTmp.Sounds[(int)soundsIndex] =
            !GameSystem.ConfigTmp.Sounds[(int)soundsIndex];
        ShowOnOff();
    }

    public void ShowOnOff()
    {
        var s = GameSystem.ConfigTmp.Sounds[(int)soundsIndex] ? "On" : "Off";
        valueTmp.text = s;
    }
}
