public class TopPanelTitle : TopPanel
{
    protected override void Awake()
    {
        base.Awake();
        storyTexts[0].text =
            @"新しく手に入れたAI君に「ゲームで大戦しよ」ってお願いしたの・・・

そうしたら、なんか始まっちゃったの（汗

対戦の間違いだったんだけど・・・やばくない？";
        storyTexts[1].text =
            @"AI君：　私達の攻撃をかいくぐってゴールにたどりつければあなたの勝ちです";
        storyTexts[2].text =
            @"こうして本気とも茶番ともつかぬ戦いが始まった・・・";
    }
}
