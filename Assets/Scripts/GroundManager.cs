using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] GameObject[] groundsGO;
    [SerializeField] Ground[] grounds;
    Dictionary<GameObject, Ground> groundsGoToGrounds = new();

    void Awake()
    {
        grounds = GetComponentsInChildren<Ground>();
        groundsGO = new GameObject[grounds.Length];
        for (int i=0; i<groundsGO.Length; i++)
        {
            groundsGO[i] = grounds[i].gameObject;
            groundsGoToGrounds.Add(groundsGO[i], grounds[i]);
        }
    }
    public Ground GetGround(GameObject go)
    {
        return groundsGoToGrounds[go];
    }
}
