using UnityEngine;

public class ScreenSettingsPanel : Panel, ICancelable
{
    [SerializeField] OptionMenuPanel optionMenuPanel;
    [SerializeField] ScreenModeItem screenModeItem;
    [SerializeField] ResolutionItem resolutionItem;
    [SerializeField] VsyncItem vsyncItem;
    [SerializeField] FrameRateItem frameRateItem;
    [SerializeField] UIManager uiManager;

    protected override void OnEnable()
    {
        base.OnEnable();
        try
        {
            screenModeItem.ShowScreenMode();
            resolutionItem.Interactable = !GameSystem.ConfigTmp.IsFullScreen;
            resolutionItem.ShowResolution(GameSystem.ConfigTmp.CurResolutionIndex);
            vsyncItem.ShowVsync();
            frameRateItem.Interactable = !GameSystem.ConfigTmp.IsVsync;
            frameRateItem.ShowFrameRate(GameSystem.ConfigTmp.CurFrameRateIndex);
        }
        catch
        {
            optionMenuPanel.ConfigInit();
            screenModeItem.ShowScreenMode();
            resolutionItem.Interactable = !GameSystem.ConfigTmp.IsFullScreen;
            resolutionItem.ShowResolution(GameSystem.ConfigTmp.CurResolutionIndex);
            vsyncItem.ShowVsync();
            frameRateItem.Interactable = !GameSystem.ConfigTmp.IsVsync;
            frameRateItem.ShowFrameRate(GameSystem.ConfigTmp.CurFrameRateIndex);
        }
        uiManager.SetCurPanel(this);
    }

    public void Cancel()
    {
        optionMenuPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }    
}
