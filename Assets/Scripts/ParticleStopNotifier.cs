using System;
using UnityEngine;

public class ParticleStopNotifier : MonoBehaviour
{
    [SerializeField] ParticleType particleType;
    public event Action<ParticleType> Stopped;

    void OnParticleSystemStopped()
    {
        Stopped?.Invoke(particleType);
    }
}
