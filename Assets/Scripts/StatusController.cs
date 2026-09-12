using UnityEngine;

public abstract class StatusController : MonoBehaviour
{
    public virtual int MaxHp { get; set; }
    public virtual int Hp { get; set; }
    public virtual bool IsHpZero => false;
    public virtual void Die(DeadType deadType) { }
    public virtual void RaiseHpChanged() { }
    public virtual void RaisePause() { }
}
