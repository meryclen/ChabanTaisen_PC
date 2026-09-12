[System.Serializable]
public class GameData
{
    public int CurStage;
    public int CurCheckPoint;
    public int MaxHp;
    public int CurStageClearTime;

    readonly public int StageDatasNum = 2;
    public DifficultyData[] DifficultyDatas;

    public GameData()
    {
        CurStage = 1;
        MaxHp = 100;
        CurStageClearTime = 300;
        DifficultyDatas = new DifficultyData[(int)Difficulty.DifficultyNum];
        for (int i=0; i<(int)Difficulty.DifficultyNum; i++)
        {
            DifficultyDatas[i] = new();
            Difficulty difficulty = Difficulty.DifficultyNum; //‰Šú‰»
            switch (i)
            {
                case 0:
                    difficulty = Difficulty.Easy;
                    break;
                case 1:
                    difficulty = Difficulty.Normal;
                    break;
                case 2:
                    difficulty = Difficulty.Hard;
                    break;
            }
            DifficultyDatas[i].Difficulty = difficulty;
            DifficultyDatas[i].StageDatas = new StageData[StageDatasNum];
            for (int j=0; j < StageDatasNum; j++)
            {
                DifficultyDatas[i].StageDatas[j] = new();
                DifficultyDatas[i].StageDatas[j].BestStageClearTime = 300;
            }
        }
    }
}

[System.Serializable]
public class StageData
{
    public int Stage;
    public int BestStageClearTime;
}

[System.Serializable]
public class DifficultyData
{
    public Difficulty Difficulty;
    public StageData[] StageDatas;
}
