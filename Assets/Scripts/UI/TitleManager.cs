using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject startButton;
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] GameObject resultPanel;

    string path;
    int soundIndexNum;


    void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "config.json");
        var inputSystemManager = new InputSystemManager(null);
        inputSystemManager.SetState(GameState.Title);
        GameSystem.CreateGameData();

        GameSystem.resolutions = new ScreenResolution[]
        {
            new ScreenResolution
            {
                Index = 0,
                Width = 1920,
                Height = 1080
            },
            new ScreenResolution
            {
                Index = 1,
                Width = 1600,
                Height = 900
            },
            new ScreenResolution
            {
                Index = 2,
                Width = 1280,
                Height = 720
            }
        };
        GameSystem.targetFrameRates = new TargetFrameRate[]
        {
            new TargetFrameRate
            {
                Index = 0,
                FrameRate = 60
            },
            new TargetFrameRate
            {
                Index = 1,
                FrameRate = 120
            },
            new TargetFrameRate
            {
                Index = 2,
                FrameRate = -1
            }
        };
        GameSystem.Resolutions_num = GameSystem.resolutions.Length;
        GameSystem.TargetFrameRates_num = GameSystem.targetFrameRates.Length;

        soundIndexNum = (int)SoundIndex.SoundIndexNum;
        try
        {
            ReadJson();
        }
        catch (Exception)
        {
            var config = new Config();
            
            config.Sounds = new bool[soundIndexNum];
            for (int i = 0; i < soundIndexNum; i++)
            {
                config.Sounds[i] = true;
            }
            GameSystem.Config = config;
            string json = JsonUtility.ToJson(config, true);
            File.WriteAllText(path, json);
        }
        GameSystem.ConfigTmp = new Config(GameSystem.Config);
        optionMenuPanel.ScreenInit();
    }
    void ReadJson()
    {
        var json = File.ReadAllText(path);
        var config = JsonUtility.FromJson<Config>(json);
        if (config.Sounds.Length != soundIndexNum)
        {
            throw new Exception();
        }
        GameSystem.Config = config;
    }

    public void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(startButton);
    }    
}
