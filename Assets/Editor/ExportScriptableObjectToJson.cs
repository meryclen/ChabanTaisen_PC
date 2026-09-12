using System.IO;
using UnityEditor;
using UnityEngine;

public static class ExportJson
{
    [MenuItem("Tools/Export Selected ScriptableObject To Json")]
    static void ExportSelected()
    {
        ScriptableObject so =
            Selection.activeObject as ScriptableObject;

        if (so == null)
        {
            Debug.LogError("ScriptableObjectÇëIëÇµÇƒÇ≠ÇæÇ≥Ç¢ÅB");
            return;
        }

        string json = JsonUtility.ToJson(so, true);
        string path =
            Path.Combine(
                Application.dataPath,
                "Json");
        path =
            Path.Combine(
                path,
                so.name + ".json");

        File.WriteAllText(path, json);

        Debug.Log(path + " Ç…ï€ë∂ÇµÇ‹ÇµÇΩÅB");
    }
}
