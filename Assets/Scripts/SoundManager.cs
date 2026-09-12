using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip audioClip;
    [SerializeField] SoundIndex soundsIndex;

    protected virtual void Awake()
    {
        audioSource.enabled = GameSystem.Config.Sounds[(int)soundsIndex];
    }

    public void Raise()
    {
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
