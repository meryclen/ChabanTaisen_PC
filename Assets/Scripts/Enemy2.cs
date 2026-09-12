using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    [SerializeField] Transform turret;
    [SerializeField] Transform barrel;

    [SerializeField] StatusAI statusAI;
    [SerializeField] ImmovableAI immovableAI;
    [SerializeField] FireAI fireAI;
    [SerializeField] DamageController damageController;
    [SerializeField] FlashController flashController;

    List<ITickable> tickables = new();

    GameObject player;

    protected override void Awake()
    {
        base.Awake();

        player = GameObject.FindWithTag("Player");

        statusAI.Initialize(this, null);
        immovableAI.Initialize(statusAI);
        fireAI.Initialize(statusAI, null, immovableAI);
        damageController.Initialize(this, null, null, null);

        immovableAI.Target = player.transform;

        tickables.Add(immovableAI);
        tickables.Add(fireAI);
    }

    void Update()
    {
        immovableAI.Tick();
        if (statusAI.CurStatus == EnemyStatus.Attack) fireAI.Tick();
    }
}
