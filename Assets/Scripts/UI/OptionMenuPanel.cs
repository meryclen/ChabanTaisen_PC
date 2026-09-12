using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class OptionMenuPanel : Panel, ICancelable
{
    [SerializeField] GameObject startMenuPanel;
    string path;
    bool isUpdated;
    public bool IsUpdated { get => isUpdated; set => isUpdated = value; }
    [SerializeField] UIManager uiManager;


    protected override void Awake()
    {
        path = Path.Combine(Application.persistentDataPath, "config.json");
    }    
    protected override void OnEnable()
    {
        base.OnEnable();
        GameSystem.ConfigTmp.CopyFrom(GameSystem.Config);
        uiManager.SetCurPanel(this);
    }
    
    public void Cancel()
    {
        if (isUpdated) WriteJson();
        startMenuPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void WriteJson()
    {
        GameSystem.Config.CopyFrom(GameSystem.ConfigTmp);
        ScreenInit();
        string json = JsonUtility.ToJson(GameSystem.Config, true);
        File.WriteAllText(path, json);
    }

    public void ScreenInit()
    {
        if (GameSystem.Config.IsVsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = GameSystem.targetFrameRates[GameSystem.Config.CurFrameRateIndex].FrameRate;
        }

        if (GameSystem.Config.IsFullScreen)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(
                GameSystem.resolutions[GameSystem.Config.CurResolutionIndex].Width,
                GameSystem.resolutions[GameSystem.Config.CurResolutionIndex].Height,
                FullScreenMode.Windowed);
        }
    }

    public void ConfigInit()
    {
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
    }
}
