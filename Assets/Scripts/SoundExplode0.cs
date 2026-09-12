using System;

public class SoundExplode0 : SoundManager
{
    public event Action OnExplode;

    void OnEnable()
    {
        OnExplode += Raise;
    }
    protected override void Awake()
    {
        base.Awake();
    }
    void OnDisable()
    {
        OnExplode -= Raise;
    }
    public void RaiseExplode()
    {
        OnExplode?.Invoke();
    }
}
