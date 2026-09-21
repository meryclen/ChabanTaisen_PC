using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LoadDefaultSettingsButton : MonoBehaviour
{
    //[SerializeField] Config debugConfig;
    //[SerializeField] Config debugConfigTmp;

    [SerializeField] OptionMenuPanel optionMenuPanel;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        Load();
        optionMenuPanel.IsUpdated = true;
    }

    void Load()
    {
        var config = new Config();
        int soundIndexNum = (int)SoundIndex.SoundIndexNum;
        config.Sounds = new bool[soundIndexNum];
        for (int i = 0; i < soundIndexNum; i++)
        {
            config.Sounds[i] = true;
        }
        GameSystem.ConfigTmp = config;
        GameSystem.Config = new Config(config);
        //debugConfig = GameSystem.Config;
        //debugConfigTmp = GameSystem.ConfigTmp;
    }
}
