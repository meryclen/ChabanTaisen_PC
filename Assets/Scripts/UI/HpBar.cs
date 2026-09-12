using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] StatusPlayer statusPlayer;
    [SerializeField] RectTransform maxHpRect;
    [SerializeField] Image curHp;
    float pixelsPerHp;

    void OnEnable()
    {
        pixelsPerHp = maxHpRect.sizeDelta.x / statusPlayer.MaxHp;
        statusPlayer.OnHpChanged += Refresh;
    }
    void OnDisable()
    {
        statusPlayer.OnHpChanged -= Refresh;
    }
    public void Refresh()
    {
        curHp.fillAmount = (float)statusPlayer.Hp / statusPlayer.MaxHp;
        Log.D(curHp.fillAmount.ToString());
    }
}
