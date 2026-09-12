using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResolutionItem : MonoBehaviour, IPointerClickHandler, IMoveHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueTmp;
    [SerializeField] OptionMenuPanel optionMenuPanel;
    CanvasGroup canvasGroup;

    bool interactable = true;
    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = value ? 1f : 0.25f;
            }
        }
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Interactable = !GameSystem.ConfigTmp.IsFullScreen;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
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
        if (!interactable) return;
        if (eventData.moveDir == MoveDirection.Left) LeftButton();
        if (eventData.moveDir == MoveDirection.Right) RightButton();
    }    

    void LeftButton()
    {
        var tmpIndex = GameSystem.ConfigTmp.CurResolutionIndex;
        tmpIndex += GameSystem.Resolutions_num;
        tmpIndex--;
        tmpIndex %= GameSystem.Resolutions_num;
        GameSystem.ConfigTmp.CurResolutionIndex = tmpIndex;
        ShowResolution(tmpIndex);
    }
    void RightButton()
    {
        var tmpIndex = GameSystem.ConfigTmp.CurResolutionIndex;
        tmpIndex++;
        tmpIndex %= GameSystem.Resolutions_num;
        GameSystem.ConfigTmp.CurResolutionIndex = tmpIndex;
        ShowResolution(tmpIndex);
    }

    public void ShowResolution(int curResolutionIndex)
    {
        var s = GameSystem.resolutions[curResolutionIndex].Width.ToString() +
            " * " +
            GameSystem.resolutions[curResolutionIndex].Height.ToString();
        valueTmp.text = s;
    }
}
