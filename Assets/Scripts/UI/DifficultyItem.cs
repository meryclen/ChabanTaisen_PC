using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultyItem : MonoBehaviour, IPointerClickHandler, IMoveHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueTmp;
    [SerializeField] GameObject cancelButton;

    readonly string[] difficultyStrings = new string[] { "Easy", "Normal", "Hard" };
    
    void OnEnable()
    {
        try
        {
            ShowDifficulty((int)GameSystem.ConfigTmp.Difficulty);
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
            ShowDifficulty((int)GameSystem.ConfigTmp.Difficulty);
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
            EventSystem.current.SetSelectedGameObject(cancelButton);
        }
    }    

    void LeftButton()
    {
        var tmpIndex = (int)GameSystem.ConfigTmp.Difficulty;
        tmpIndex += (int)Difficulty.DifficultyNum;
        tmpIndex--;
        tmpIndex %= (int)Difficulty.DifficultyNum;
        GameSystem.ConfigTmp.Difficulty = (Difficulty)tmpIndex;
        ShowDifficulty(tmpIndex);
    }
    void RightButton()
    {
        var tmpIndex = (int)GameSystem.ConfigTmp.Difficulty;
        tmpIndex++;
        tmpIndex %= (int)Difficulty.DifficultyNum;
        GameSystem.ConfigTmp.Difficulty = (Difficulty)tmpIndex;
        ShowDifficulty(tmpIndex);
    }

    void ShowDifficulty(int index)
    {
        var s = difficultyStrings[index];
        valueTmp.text = s;
    }
}
