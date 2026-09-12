using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : SoundManager
{
    [SerializeField] AudioClip damage0;
    [SerializeField] AudioClip damage1;
    [SerializeField] AudioClip falldown0;
    [SerializeField] AudioClip falldown1;
    [SerializeField] AudioClips[] footSteps;
    
    Dictionary<SurfaceType, FootStepsType> surfaceToStep = new();
    [SerializeField] StatusPlayer statusPlayer;
    bool alive = true;

    enum FootStepsType
    {
        Soil,
        Stone
    }

    protected override void Awake()
    {
        base.Awake();
        surfaceToStep.Add(SurfaceType.Soil, FootStepsType.Soil);
        surfaceToStep.Add(SurfaceType.Stone, FootStepsType.Stone);
    }
    public void Damage()
    {
        if (!alive) return;
        int rnd = Random.Range(0, 2);
        audioSource.PlayOneShot(rnd == 0 ? damage0 : damage1);
        if (statusPlayer.PlayerStatus == PlayerStatus.Dead) alive = false;
    }
    public void FallDown()
    {
        if (!alive) return;
        int rnd = Random.Range(0, 2);
        audioSource.PlayOneShot(rnd == 0 ? falldown0 : falldown1);
        if (statusPlayer.PlayerStatus == PlayerStatus.Dead) alive = false;
    }
    public void FootSteps(SurfaceType surfaceType)
    {
        if (!alive) return;

        int index = (int)surfaceToStep[surfaceType];
        int rnd = Random.Range(0, index);
        audioSource.PlayOneShot(footSteps[index].audioClips[rnd]);

        if (statusPlayer.PlayerStatus == PlayerStatus.Dead) alive = false;
    }
}

[System.Serializable]
public class AudioClips
{
    [SerializeField] public AudioClip[] audioClips;
}
