using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/DifficultySettings")]
public class StageSettings : ScriptableObject
{
    [SerializeField] public DifficultySettings[] difficulties;
}
