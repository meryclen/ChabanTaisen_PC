using UnityEngine.EventSystems;

public class UISoundNormal : UISound
{    
    public override void OnSubmit(BaseEventData eventData)
    {
        audioSource.PlayOneShot(ok);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        audioSource.PlayOneShot(ok);
    }
}
