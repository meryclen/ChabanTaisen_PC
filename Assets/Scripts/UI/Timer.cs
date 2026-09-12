using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] StatusPlayer statusPlayer;
    [SerializeField] TextMeshProUGUI tmp_m;
    [SerializeField] TextMeshProUGUI tmp_colon;
    [SerializeField] TextMeshProUGUI tmp_s0;
    [SerializeField] TextMeshProUGUI tmp_s1;
    [SerializeField] float timer;
    [SerializeField] float remainTimer;
    int timer_int;
    [SerializeField] float timerLimit = 60f * 4f;
    [SerializeField] float timerWarning = 60f;
    bool isTimerWarning;
    int previousSeconds;
    bool isTimerOver;
    readonly string[] ints = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    void OnEnable()
    {
        statusPlayer.OnWin += RefreshWin;
        tmp_m.color = new Color(1f, 1f, 1f, 0.25f);
        tmp_s0.color = new Color(1f, 1f, 1f, 0.25f);
        tmp_s1.color = new Color(1f, 1f, 1f, 0.25f);
    }
    void OnDisable()
    {
        statusPlayer.OnWin -= RefreshWin;
    }

    void Update()
    {
        if (isTimerOver) return;
        timer += Time.deltaTime;
        remainTimer = timerLimit - timer;
        
        if (!isTimerWarning && remainTimer < timerWarning)
        {
            isTimerWarning = true;
            tmp_m.color = new Color(1f, 0f, 0f, 0.25f);
            tmp_colon.color = new Color(1f, 0f, 0f, 0.25f);
            tmp_s0.color = new Color(1f, 0f, 0f, 0.25f);
            tmp_s1.color = new Color(1f, 0f, 0f, 0.25f);
        }
        if (timer >= timerLimit)
        {
            statusPlayer.Die(DeadType.TimeOver);
            isTimerOver = true;
            return;
        }
        
        timer_int = (int)remainTimer;
        int timer_int_m = timer_int / 60;
        int timer_int_s = timer_int % 60;

        if (timer_int_s != previousSeconds)
        {            
            previousSeconds = timer_int_s;
            tmp_m.text = ints[timer_int_m];
            tmp_s0.text = ints[timer_int_s / 10];
            tmp_s1.text = ints[timer_int_s % 10];
        }        
    }

    public void RefreshWin()
    {
        GameSystem.GameData.CurStageClearTime = (int)timer;
    }
}
