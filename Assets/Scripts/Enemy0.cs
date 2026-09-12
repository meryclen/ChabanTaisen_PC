using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy0 : Enemy
{
    [SerializeField] Transform turret;
    [SerializeField] Transform barrel;

    [SerializeField] StatusAI statusAI;
    [SerializeField] MoveAI moveAI;
    [SerializeField] FireAI fireAI;
    [SerializeField] GroundMoter groundMoter;
    [SerializeField] DamageController damageController;
    [SerializeField] FlashController flashController;

    List<ITickable> tickables = new();

    GameObject player;

    Vector3 desiredDir;
    Vector3 lastForward;

    bool isGreater;
    public override bool IsGreater { set => isGreater = value; }


    protected override void Awake()
    {
        base.Awake();

        player = GameObject.FindWithTag("Player");

        statusAI.Initialize(this, moveAI);
        moveAI.Initialize(statusAI, groundMoter);
        fireAI.Initialize(statusAI, moveAI, null);
        if (groundMoter is GroundAI ga) ga.Initialize(moveAI);
        damageController.Initialize(this, moveAI, groundMoter, null);
        moveAI.Target = player.transform;

        tickables.Add(moveAI);
        tickables.Add(fireAI);
        tickables.Add(groundMoter);
        
        desiredDir = transform.forward;
        lastForward = transform.forward;
    }
    
    void Update()
    {
        moveAI.Tick();
        groundMoter.Tick();

        Vector3 pos = transform.position;
        desiredDir = groundMoter.Move;
        pos += groundMoter.Move * Time.deltaTime;
        Vector3 normalV = Vector3.zero;
        int count = 0;

        if (groundMoter is GroundAI groundAI)
        {
            foreach (var probe in groundAI.probes)
            {
                if (!probe.hit) continue;
                normalV += probe.normal;
                count++;
            }
        }

        bool rotation_able = true;
        transform.position = pos;

        if (moveAI.Agent.enabled)
        {
            moveAI.Agent.nextPosition = pos;
        }
        else
        {
            rotation_able = false;
            if (groundMoter.gravityDamageInfo.DamageType == DamageType.None)
            {
                if (NavMesh.SamplePosition(pos, out var hit, 1f, NavMesh.AllAreas))
                {
                    moveAI.Agent.Warp(hit.position);
                    moveAI.Agent.enabled = true;
                }
                else
                {
                    if (transform.position.y < -25f) statusAI.Die(DeadType.DropOut);
                }
            }
        }

        if (rotation_able)
        {
            Quaternion prevRot = transform.rotation;

            if (count != 0)
            {
                normalV /= count;
                normalV.Normalize();

                Vector3 lastForwardTmp =
                    Vector3.ProjectOnPlane(
                        desiredDir,
                        normalV).normalized;
                
                if (lastForwardTmp.sqrMagnitude > 0.01f)
                {
                    lastForward = lastForwardTmp;
                }

                Quaternion targetRot =
                    Quaternion.LookRotation(
                        lastForward,
                        normalV);

                transform.rotation =
                    Quaternion.Slerp(
                        prevRot,
                        targetRot,
                        90f * Time.deltaTime);
            }
        }
        if (statusAI.CurStatus == EnemyStatus.Attack) fireAI.Tick();
        groundMoter.Move = Vector3.zero;
    }
}
