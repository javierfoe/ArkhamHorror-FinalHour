using System;
using System.Collections.Generic;

public class CthulhuOmen : AncientOneOmen
{
    private const string CthluhuPath = "Cthulhu";
    private readonly List<Monster> _deepOnes = new(), _starSpawns = new();

    protected override string EldritchHorrorResource => CthluhuPath;

    public CthulhuOmen(Difficulty difficulty) : base(difficulty)
    {
    }

    protected override void AddMonsters(EldritchMinion eldritchMinion, List<Monster> monsters)
    {
        switch (eldritchMinion)
        {
            case EldritchMinion.DeepOne:
                _deepOnes.AddRange(monsters);
                break;
            case EldritchMinion.StarSpawn:
                _starSpawns.AddRange(monsters);
                break;
        }
    }

    protected override void ActivateInterval(int index)
    {
        throw new System.NotImplementedException();
    }
}