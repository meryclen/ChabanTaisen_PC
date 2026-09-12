using System;
using System.IO;
using UnityEngine;

public static class GameSystem
{
    static GameData gameData;
    static Config config;
    public static Config Config { get => config; set => config = value; }
    public static Config ConfigTmp;
    public static GameData GameData => gameData;
    static SaveUtility saveUtility;
    public static SaveUtility SaveUtility => saveUtility;

    public static ScreenResolution[] resolutions;
    public static int Resolutions_num { get; set; }
    public static TargetFrameRate[] targetFrameRates;
    public static int TargetFrameRates_num { get; set; }

    public static void CreateGameData()
    {
        var savePath = Path.Combine(Application.persistentDataPath, "save.dat");
        if (saveUtility == null) saveUtility = new SaveUtility();

        try
        {
            gameData = saveUtility.Load<GameData>(savePath);
        }        
        catch (Exception)
        {
            gameData = new GameData();
            saveUtility.Save(savePath, gameData);
        }
    }
}
