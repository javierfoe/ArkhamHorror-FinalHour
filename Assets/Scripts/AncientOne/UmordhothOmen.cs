using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmordhothOmen : AncientOneOmen
{
    private const string UmordhothPath = "Umôrdhoth";
    private readonly List<Monster> _ghouls = new();

    protected override string EldritchHorrorResource => UmordhothPath;

    public UmordhothOmen(Difficulty difficulty, ArkhamHorror arkhamHorror) : base(difficulty, arkhamHorror)
    {
    }

    protected override void AddMonsters(EldritchMinion eldritchMinion, List<Monster> monsters)
    {
        if (eldritchMinion != EldritchMinion.GhoulHorde) return;
        _ghouls.AddRange(monsters);
    }

    protected override IEnumerator ActivateInterval(int symbols)
    {
        switch (symbols)
        {
            case > 5:
                var random = Random.Range(0, _ghouls.Count - 1);
                var building = _ghouls[random].Location.Building;
                yield return ArkhamHorror.SpawnMonsters(2, building);
                break;
            case > 2:
                ArkhamHorror.RemoveRandomSeal();
                break;
            case > 0:
                break;
        }
    }
}