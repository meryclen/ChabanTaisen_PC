using UnityEngine;

public class Explode0 : Explode
{
    ParticleSystem particle;
    [SerializeField] SoundManager soundManager;

    protected override void Awake()
    {
        base.Awake();
        particle = GetComponent<ParticleSystem>();
    }
    
    void OnEnable()
    {
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particle.Play();
    }
    
    void OnParticleSystemStopped()
    {
        Pool.Return(this);
    }
    
    public void Explode(SurfaceType surfaceType)
    {
        if (surfaceType == SurfaceType.Metal) soundManager.Raise();
    }
}
