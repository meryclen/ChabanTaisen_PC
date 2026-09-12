using UnityEngine;

public class DifficultySettingsPanel : Panel, ICancelable
{
    [SerializeField] GameObject optionMenuPanel;
    [SerializeField] DifficultyItem difficultyItem;        
    [SerializeField] UIManager uiManager;

    protected override void OnEnable()
    {
        base.OnEnable();        
        uiManager.SetCurPanel(this);
    }

    public void Cancel()
    {
        optionMenuPanel.SetActive(true);
        gameObject.SetActive(false);
    }    
}
