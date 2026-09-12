using UnityEngine;

public class ClearManager : MonoBehaviour
{
    void Awake()
    {
        var inputSystemManager = new InputSystemManager(null);
        inputSystemManager.SetState(GameState.Title);
    }
}
