using System.Collections.Generic;

public class ShuddeMellOmen : AncientOneOmen
{
    private const string ShuddeMellPath = "ShuddeM'ell";
    private readonly List<Monster> _chthonians = new();

    protected override string EldritchHorrorResource => ShuddeMellPath;

    public ShuddeMellOmen(Difficulty difficulty) : base(difficulty)
    {
    }

    protected override void AddMonsters(EldritchMinion eldritchMinion, List<Monster> monsters)
    {
        if (eldritchMinion != EldritchMinion.Chthonian) return;
        _chthonians.AddRange(monsters);
    }

    protected override void ActivateInterval(int index)
    {
        throw new System.NotImplementedException();
    }
}