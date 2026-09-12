using System;
using UnityEngine;

public class SoundGrenade0 : SoundManager
{
    public event Action OnFire;
    
    void OnEnable()
    {
        OnFire += Raise;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    void OnDisable()
    {
        OnFire -= Raise;
    }

    public void RaiseFire()
    {
        OnFire?.Invoke();
    }
}
