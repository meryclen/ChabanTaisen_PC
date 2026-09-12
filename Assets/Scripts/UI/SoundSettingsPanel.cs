using System;
using UnityEngine;

public class SoundSettingsPanel : Panel, ICancelable
{
    [SerializeField] OptionMenuPanel optionMenuPanel;

    [SerializeField] OnOffItem playerVoiceItem;
    [SerializeField] OnOffItem playerAttackItem;
    [SerializeField] OnOffItem playerFootStepsItem;
    [SerializeField] OnOffItem enemyAttackItem;

    [SerializeField] UIManager uiManager;    
    
    protected override void OnEnable()
    {
        base.OnEnable();
        try
        {
            playerVoiceItem.ShowOnOff();
            playerAttackItem.ShowOnOff();
            playerFootStepsItem.ShowOnOff();
            enemyAttackItem.ShowOnOff();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            optionMenuPanel.ConfigInit();
            playerVoiceItem.ShowOnOff();
            playerAttackItem.ShowOnOff();
            playerFootStepsItem.ShowOnOff();
            enemyAttackItem.ShowOnOff();
        }
        uiManager.SetCurPanel(this);
    }

    public void Cancel()
    {
        optionMenuPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }    
}
