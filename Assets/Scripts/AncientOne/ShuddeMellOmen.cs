using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ShuddeMellOmen : AncientOneOmen
{
    private const string ShuddeMellPath = "ShuddeM'ell";
    private readonly List<Monster> _chthonians = new();
    private readonly MonsterDefinition _chthonian;
    private readonly int _maxChthonians;

    protected override string EldritchHorrorResource => ShuddeMellPath;

    public ShuddeMellOmen(Difficulty difficulty, ArkhamHorror arkhamHorror) : base(difficulty, arkhamHorror)
    {
        _chthonian = EldritchHorror.GetEldritchMinion(EldritchMinion.Chthonian).monsterDefinition;
        _maxChthonians = _chthonian.amount;
    }

    protected override void AddMonsters(EldritchMinion eldritchMinion, List<Monster> monsters)
    {
        if (eldritchMinion != EldritchMinion.Chthonian) return;
        _chthonians.AddRange(monsters);
    }

    protected override IEnumerator ActivateInterval(int symbols)
    {
        switch (symbols)
        {
            case > 6:
                yield return ArkhamHorror.DamageRitual(2);
                break;
            case > 4:
                break;
            case > 2:
                break;
            case > 0:
                yield return SpawnChthonian();
                if (_chthonians.Count == _maxChthonians)
                {
                    var random = Random.Range(0, _chthonians.Count - 1);
                    yield return _chthonians[random].Activate();
                }
                break;
        }
    }

    private IEnumerator SpawnChthonian()
    {
        if (_chthonians.Count < _maxChthonians)
        {
            var spawn = ArkhamHorror.MonsterSpawn(_chthonian);
            var monster = spawn.Monster;
            ArkhamHorror.IncomingMonsterLowestGate(monster);
            _chthonians.Add(monster);
            yield return spawn;
        }
    }
}