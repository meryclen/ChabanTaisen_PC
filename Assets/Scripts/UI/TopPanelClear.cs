public class TopPanelClear : TopPanel
{
    protected override void Awake()
    {
        base.Awake();
        int rnd = UnityEngine.Random.Range(0, 2);
        switch (rnd)
        {
            case 0:
                storyTexts[0].text =
                    @"AI君：　よくぞたどり着きました、あなたの勝ちです";
                break;
            case 1:
                storyTexts[0].text =
                    @"AI君：　まいりました、今回は私の負けです";
                break;
        }
        rnd = UnityEngine.Random.Range(0, 2);
        switch (rnd)
        {
            case 0:
                storyTexts[1].text =
                    @"AI君：　あなたの動きを読めませんでした";
                break;
            case 1:
                storyTexts[1].text =
                    @"AI君：　次の大戦をお待ちしております";
                break;
        }
    }
}
