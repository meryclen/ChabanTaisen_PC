using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] GameObject startMenuPanel;
    [SerializeField] GameObject startMenuButton;
    [SerializeField] InputActionReference cancel;
    [SerializeField] TextMeshProUGUI[] tmps;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(startMenuButton);
        Show();
        cancel.action.performed += OnCancel;
    }
    void OnDisable()
    {
        cancel.action.performed -= OnCancel;
    }
    void Show()
    {
        if (GameSystem.GameData == null) return;

        for (int i = 0; i < (int)Difficulty.DifficultyNum; i++)
        {
            string str = null;
            string difficultyStr = null;
            switch (i)
            {
                case 0:
                    difficultyStr = "Easy";
                    break;
                case 1:
                    difficultyStr = "Normal";
                    break;
                case 2:
                    difficultyStr = "Hard";
                    break;
            }
            str += difficultyStr;
            str += "\r\n";
            str += "\r\n";
            for (int j = 0; j < GameSystem.GameData.StageDatasNum; j++)
            {
                str += "ステージ";
                str += (j+1).ToString();
                str += "\r\n";

                int timer_int_m =
                    GameSystem.GameData.DifficultyDatas[i].StageDatas[j].BestStageClearTime / 60;
                int timer_int_s =
                    GameSystem.GameData.DifficultyDatas[i].StageDatas[j].BestStageClearTime % 60;

                string timer_s_m;
                if (timer_int_m < 10) timer_s_m = '0' + timer_int_m.ToString();
                else timer_s_m = timer_int_m.ToString();

                string timer_s_s;
                if (timer_int_s < 10) timer_s_s = '0' + timer_int_s.ToString();
                else timer_s_s = timer_int_s.ToString();

                str += timer_s_m + ':' + timer_s_s;
                str += "\r\n";
                str += "\r\n";
            }
            tmps[i].text = str;
        }        
    }

    void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed) Cancel();
    }
    void Cancel()
    {
        startMenuPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
