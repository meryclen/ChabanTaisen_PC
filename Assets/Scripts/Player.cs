using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] Fire fire;
    [SerializeField] GroundMoter groundMoter;
    [SerializeField] Anim anim;
    [SerializeField] Pistol2 pistol2;
    [SerializeField] Line line;
    [SerializeField] DamageController damageController;
    [SerializeField] FlashController flashController;
    [SerializeField] StatusPlayer statusPlayer;

    List<ITickable> tickables = new();    

    void Awake()
    {
        inputManager.Initialize(this, groundMoter, anim);
        fire.Initialize(inputManager, anim);
        anim.Initialize(inputManager, groundMoter, line);
        pistol2.Initialize(anim);
        line.Initialize();
        damageController.Initialize(null, null, groundMoter, anim);
    }
    void Start()
    {
        tickables.Add(inputManager);
        tickables.Add(fire);
        tickables.Add(groundMoter);
        tickables.Add(pistol2);
        tickables.Add(anim);
    }

    void Update()
    {
        if (statusPlayer.PlayerStatus != PlayerStatus.Dead)
        {
            foreach (var t in tickables)
            {
                t.Tick();
            }
        }
        else
        {
            groundMoter.Tick();
            anim.Tick();
        }
        transform.position += groundMoter.Move * Time.deltaTime;
        groundMoter.Move = Vector3.zero;
    }
    void LateUpdate()
    {
        if (!statusPlayer.IsHpZero)
        {
            foreach (var t in tickables)
            {
                t.LateTick();
            }
        }
        else
        {
            anim.LateTick();
        }
    }
}
