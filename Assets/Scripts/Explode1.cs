using UnityEngine;

public class Explode1 : Explode
{
    [SerializeField] ParticleSystem fire;
    [SerializeField] ParticleSystem smoke;
    bool isFireStopped;
    bool isSmokeStopped;
    [SerializeField] SoundManager soundManager;

    protected override void Awake()
    {
        base.Awake();
        fire.GetComponent<ParticleStopNotifier>().Stopped += OnParticleStopped;
        smoke.GetComponent<ParticleStopNotifier>().Stopped += OnParticleStopped;
    }

    void OnEnable()
    {
        fire.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        fire.Play();
        smoke.Play();
    }
    void OnDestroy()
    {
        fire.GetComponent<ParticleStopNotifier>().Stopped -= OnParticleStopped;
        smoke.GetComponent<ParticleStopNotifier>().Stopped -= OnParticleStopped;
    }

    void OnParticleStopped(ParticleType particleType)
    {
        if (particleType == ParticleType.Fire) isFireStopped = true;
        if (particleType == ParticleType.Smoke) isSmokeStopped = true;
        if (isFireStopped && isSmokeStopped)
        {
            isFireStopped = false;
            isSmokeStopped = false;
            Pool.Return(this);
        }
    }

    public void Explode()
    {
        soundManager.Raise();
    }
}
