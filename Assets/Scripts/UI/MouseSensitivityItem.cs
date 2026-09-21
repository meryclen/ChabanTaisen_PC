using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseSensitivityItem : MonoBehaviour, IPointerClickHandler, IMoveHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueTmp;
    [SerializeField] GameObject upItem;
    [SerializeField] GameObject downItem;

    readonly string[] mouseSensitivityStrings = new string[] {
        "-50%", "-40%", "-30%", "-20%", "-10%", "Å}0%",
        "+10%", "+20%", "+30%", "+40%", "+50%" };

    int mouseSensitivityIndex;
    [SerializeField] MouseSensitivityType mouseSensitivityType;

    void OnEnable()
    {
        if (mouseSensitivityType == MouseSensitivityType.Aim)
        {
            mouseSensitivityIndex = GameSystem.ConfigTmp.MouseSensitivityIndex;
        }
        else if (mouseSensitivityType == MouseSensitivityType.Camera)
        {
            mouseSensitivityIndex = GameSystem.ConfigTmp.MouseSensitivityIndexCamera;
        }

        try
        {
            ShowMouseSensitivity(mouseSensitivityIndex);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            var config = new Config();
            var soundIndexNum = (int)SoundIndex.SoundIndexNum;
            config.Sounds = new bool[soundIndexNum];
            for (int i = 0; i < soundIndexNum; i++)
            {
                config.Sounds[i] = true;
            }
            GameSystem.Config = config;
            string json = JsonUtility.ToJson(config, true);
            var path = Path.Combine(Application.persistentDataPath, "config.json");
            File.WriteAllText(path, json);
            GameSystem.ConfigTmp = new Config(GameSystem.Config);
            ShowMouseSensitivity(mouseSensitivityIndex);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos);
        if (localPos.x < 0) LeftButton();
        else RightButton();
    }
    public void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Left) LeftButton();
        if (eventData.moveDir == MoveDirection.Right) RightButton();
        if (eventData.moveDir == MoveDirection.Up)
        {
            EventSystem.current.SetSelectedGameObject(upItem);
        }
        if (eventData.moveDir == MoveDirection.Down)
        {
            EventSystem.current.SetSelectedGameObject(downItem);
        }
    }    

    void LeftButton()
    {
        var tmpIndex = mouseSensitivityIndex;
        tmpIndex += mouseSensitivityStrings.Length;
        tmpIndex--;
        tmpIndex %= mouseSensitivityStrings.Length;

        mouseSensitivityIndex = tmpIndex;

        if (mouseSensitivityType == MouseSensitivityType.Aim)
        {
            GameSystem.ConfigTmp.MouseSensitivityIndex = mouseSensitivityIndex;
        }
        else if (mouseSensitivityType == MouseSensitivityType.Camera)
        {
            GameSystem.ConfigTmp.MouseSensitivityIndexCamera = mouseSensitivityIndex;
        }

        var tmpIndex1 = tmpIndex - mouseSensitivityStrings.Length / 2;
        ShowMouseSensitivity(tmpIndex);

        if (mouseSensitivityType == MouseSensitivityType.Aim)
        {
            GameSystem.ConfigTmp.MouseSensitivity = 1.0f + 0.1f * tmpIndex1;
        }
        else if (mouseSensitivityType == MouseSensitivityType.Camera)
        {
            GameSystem.ConfigTmp.MouseSensitivityCamera = 1.0f + 0.1f * tmpIndex1;
        }
    }
    void RightButton()
    {
        var tmpIndex = mouseSensitivityIndex;
        tmpIndex++;
        tmpIndex %= mouseSensitivityStrings.Length;

        mouseSensitivityIndex = tmpIndex;

        if (mouseSensitivityType == MouseSensitivityType.Aim)
        {
            GameSystem.ConfigTmp.MouseSensitivityIndex = mouseSensitivityIndex;
        }
        else if (mouseSensitivityType == MouseSensitivityType.Camera)
        {
            GameSystem.ConfigTmp.MouseSensitivityIndexCamera = mouseSensitivityIndex;
        }

        var tmpIndex1 = tmpIndex - mouseSensitivityStrings.Length / 2;
        ShowMouseSensitivity(tmpIndex);

        if (mouseSensitivityType == MouseSensitivityType.Aim)
        {
            GameSystem.ConfigTmp.MouseSensitivity = 1.0f + 0.1f * tmpIndex1;
        }
        else if (mouseSensitivityType == MouseSensitivityType.Camera)
        {
            GameSystem.ConfigTmp.MouseSensitivityCamera = 1.0f + 0.1f * tmpIndex1;
        }
    }

    void ShowMouseSensitivity(int index)
    {
        var s = mouseSensitivityStrings[index];
        valueTmp.text = s;
    }
}
