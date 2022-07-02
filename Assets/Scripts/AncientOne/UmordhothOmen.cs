using System.Collections.Generic;

public class UmordhothOmen : AncientOneOmen
{
    private const string UmordhothPath = "Umôrdhoth";
    private readonly List<Monster> _ghouls = new();

    protected override string EldritchHorrorResource => UmordhothPath;

    public UmordhothOmen(Difficulty difficulty) : base(difficulty)
    {
    }

    protected override void AddMonsters(EldritchMinion eldritchMinion, List<Monster> monsters)
    {
        if (eldritchMinion != EldritchMinion.GhoulHorde) return;
        _ghouls.AddRange(monsters);
    }

    protected override void ActivateInterval(int index)
    {
        throw new System.NotImplementedException();
    }
}