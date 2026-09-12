using System;

public class SoundPistol2 : SoundManager
{
    public event Action OnFire;

    protected override void Awake()
    {
        base.Awake();
    }
    void OnEnable()
    {
        OnFire += Raise;
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
