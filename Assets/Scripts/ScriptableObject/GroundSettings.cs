using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GroundSettings")]
public class GroundSettings : ScriptableObject
{
    public float rayLength;
    public float rayLengthDead;
    public float raysLength;
    public float raysLengthDead;
    public int raysNum;
    public float rayOffset;
    public float rayOffsetDead;
    public float gravityAcceleration;
    public float gravityVelocityMax;
    public float slopeSlideLimit;
    public float slideSpeed;
    public float pushSpeed;
    public LayerMask layerMask;
    public LayerMask goalMask;

    public float spherecastRadius;
    public float skinWidth;
    public float spherecastOffset;
    public float decreaseValue;

    public float wallThreshold;
    public float hitCheckOriginOffsetY;
    public float hitPushCheckOriginOffsetY;
    public float hitPushCheckOriginOffsetY_Dead;

    public float deadValue;
    public float damageValue;
}
