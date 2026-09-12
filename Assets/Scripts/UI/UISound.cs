using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UISound : MonoBehaviour, IPointerEnterHandler, ISubmitHandler, IMoveHandler, IPointerClickHandler
{
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip choose;
    [SerializeField] protected AudioClip ok;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSource.PlayOneShot(choose);
    }
    public virtual void OnSubmit(BaseEventData eventData)
    {

    }
    public void OnMove(AxisEventData eventData)
    {
        audioSource.PlayOneShot(choose);
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }
}
