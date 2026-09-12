using UnityEngine;

public abstract class Rnd
{
    public virtual bool IsGenerate { get; set; }    
    public virtual float RndPosInterval { get; set; }
    public virtual Vector3 TargetPos { get; set; }
    public virtual float RndRotInterval { get; set; }
    public virtual Quaternion TargetRot { get; set; }
    public virtual void TargetPosGenerate() { }
    public virtual void TargetRotGenerate() { }
}

public class NeckRnd : Rnd
{
    public NeckRnd(float interval)
    {
        this.RndRotInterval = interval;
    }
    public override bool IsGenerate { get; set; }
    public override float RndRotInterval { get; set; }
    public override Quaternion TargetRot { get; set; }
    public override void TargetRotGenerate()
    {
        var rndtmp = Random.Range(0, 3);
        var pitch = -(rndtmp + 2) * RndRotInterval;
        rndtmp = Random.Range(0, 7);
        var yaw = (rndtmp - 3) * RndRotInterval;
        rndtmp = Random.Range(0, 3);
        var roll = (rndtmp - 1) * RndRotInterval;
        TargetRot = NeckBoneParentRotation *
            Quaternion.Euler(pitch, yaw, roll);
    }
    public Quaternion NeckBoneParentRotation;
    public bool IsRotStart;
}

public class NeckRnd1 : Rnd
{
    public NeckRnd1(float interval)
    {
        this.RndRotInterval = interval;
    }
    public override bool IsGenerate { get; set; }
    public override float RndRotInterval { get; set; }
    public override Quaternion TargetRot { get; set; }
    public override void TargetRotGenerate()
    {
        var rnd = Random.Range(0, 2);
        var pitch = -(rnd + 1) * RndRotInterval;
        rnd = Random.Range(0, 5);
        var yaw = (rnd - 3) * RndRotInterval;
        rnd = Random.Range(0, 2);
        var roll = ((-1 / 2) + rnd) * RndRotInterval;
        TargetRot = NeckBoneParentRotation *
            Quaternion.Euler(pitch, yaw, roll);
    }
    public Quaternion NeckBoneParentRotation;
}

public class NeckRnd2 : Rnd
{
    public NeckRnd2(float interval)
    {
        this.RndRotInterval = interval;
    }
    public override bool IsGenerate { get; set; }
    public override float RndRotInterval { get; set; }
    public override Quaternion TargetRot { get; set; }
    public override void TargetRotGenerate()
    {
        var rnd = Random.Range(0, 11);
        var pitch = (rnd - 15) * RndRotInterval / 2f;
        rnd = Random.Range(0, 11);
        var yaw = (rnd - 5) * RndRotInterval;
        rnd = Random.Range(0, 3);
        var roll = (rnd - 1) * RndRotInterval / 2f;
        LocalRot = Quaternion.Euler(pitch, yaw, roll);
        TargetRot = NeckBoneParentRotation * LocalRot;
    }
    public Quaternion LocalRot;
    public Quaternion NeckBoneParentRotation;
    public bool IsRotStart;
}

public class NeckRnd3 : Rnd
{
    public NeckRnd3(float interval)
    {
        this.RndRotInterval = interval;
    }
    public override bool IsGenerate { get; set; }
    public override float RndRotInterval { get; set; }
    public override Quaternion TargetRot { get; set; }
    public override void TargetRotGenerate()
    {
        var rnd = Random.Range(0, 11);
        var pitch = (rnd + 15) * RndRotInterval / 2f;
        rnd = Random.Range(0, 11);
        var yaw = (rnd - 5) * RndRotInterval;
        rnd = Random.Range(0, 3);
        var roll = (rnd - 1) * RndRotInterval / 2f;
        LocalRot = Quaternion.Euler(pitch, yaw, roll);
        TargetRot = NeckBoneParentRotation * LocalRot;
    }
    public Quaternion LocalRot;
    public Quaternion NeckBoneParentRotation;
    public bool IsRotStart;
}

public class DropRnd : Rnd
{
    public DropRnd(float posInterval, float rotInterval)
    {
        this.RndPosInterval = posInterval;
        this.RndRotInterval = rotInterval;
    }
    public override bool IsGenerate { get; set; }
    public override float RndPosInterval { get; set; }
    public override float RndRotInterval { get; set; }
    public override Vector3 TargetPos { get; set; }
    public override Quaternion TargetRot { get; set; }
    public override void TargetPosGenerate()
    {
        var rnd = Random.Range(0, 5);
        var x = (-2 + rnd) * RndPosInterval / 5;
        rnd = Random.Range(0, 11);
        var y = 0.4f + rnd * RndPosInterval / 2;
        rnd = Random.Range(0, 3);
        var z = (-1 + rnd) * RndPosInterval;
        TargetPos = IkTargetStartPos + new Vector3(x, y, z);
    }
    public override void TargetRotGenerate()
    {

    }
    public Vector3 IkTargetStartPos;
    public Quaternion IkTargetStartRot;
}

public class ArmRnd : Rnd
{
    public ArmRnd(float interval)
    {
        this.RndPosInterval = interval;
    }
    public override bool IsGenerate { get; set; }
    public override float RndPosInterval { get; set; }
    public override Vector3 TargetPos { get; set; }
    public override void TargetPosGenerate()
    {
        var rnd = Random.Range(0, 21);
        var x = (-10 + rnd) * RndPosInterval;
        rnd = Random.Range(0, 21);
        var z = (-10 + rnd) * RndPosInterval;
        TargetPos = OriginPos + new Vector3(x, 0f, z);
    }
    public Vector3 OriginPos;
}

public class FootRnd : Rnd
{
    public FootRnd(float interval)
    {
        this.RndPosInterval = interval;
    }
    public override bool IsGenerate { get; set; }
    public override float RndPosInterval { get; set; }
    public override Vector3 TargetPos { get; set; }
    public override void TargetPosGenerate()
    {
        var rnd = Random.Range(0, 21);
        var x = (-10 + rnd) * RndPosInterval;
        rnd = Random.Range(0, 21);
        var z = (-10 + rnd) * RndPosInterval;
        TargetPos = OriginPos + new Vector3(x, 0f, z);
    }
    public Vector3 OriginPos;
}
