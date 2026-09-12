using UnityEngine;
using UnityEditor;
using UnityEngine.Animations.Rigging;

public static class AddComponentUtility
{
    [MenuItem("Tools/Add Component To Children")]
    static void AddComponentToChildren()
    {
        GameObject parentObj = Selection.activeGameObject;
        if (parentObj == null)
        {
            Debug.LogWarning("GameObjectを選択してください");
            return;
        }

        foreach (var child in parentObj.GetComponentsInChildren<Transform>(true))
        {
            if (child == parentObj.transform) continue;
            Undo.AddComponent<MultiRotationConstraint>(child.gameObject);
        }
    }
}
