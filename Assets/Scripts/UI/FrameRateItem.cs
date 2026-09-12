using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FrameRateItem : MonoBehaviour, IPointerClickHandler, IMoveHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueTmp;
    [SerializeField] OptionMenuPanel optionMenuPanel;
    CanvasGroup canvasGroup;
    [SerializeField] GameObject downItem;
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
        if (eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(downItem);
        }
    }    

    void LeftButton()
    {
        var tmpIndex = GameSystem.ConfigTmp.CurFrameRateIndex;
        tmpIndex += GameSystem.TargetFrameRates_num;
        tmpIndex--;
        tmpIndex %= GameSystem.TargetFrameRates_num;
        GameSystem.ConfigTmp.CurFrameRateIndex = tmpIndex;
        ShowFrameRate(tmpIndex);
    }
    void RightButton()
    {
        var tmpIndex = GameSystem.ConfigTmp.CurFrameRateIndex;
        tmpIndex++;
        tmpIndex %= GameSystem.TargetFrameRates_num;
        GameSystem.ConfigTmp.CurFrameRateIndex = tmpIndex;
        ShowFrameRate(tmpIndex);
    }

    public void ShowFrameRate(int curFrameRateIndex)
    {
        string s = null;
        if (curFrameRateIndex != GameSystem.TargetFrameRates_num - 1)
        {
            s = GameSystem.targetFrameRates[curFrameRateIndex].FrameRate.ToString();
        }
        else
        {
            s = "Unlimited";
        }
        valueTmp.text = s;
    }
}
