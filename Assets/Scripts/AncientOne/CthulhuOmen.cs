using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class CthulhuOmen : AncientOneOmen
{
    private const string CthluhuPath = "Cthulhu";
    private readonly List<Monster> _deepOnes = new(), _starSpawns = new();
    private readonly int _maxDeepOnes, _maxStarSpawns;
    private readonly MonsterDefinition _deepOne, _starSpawn;

    protected override string EldritchHorrorResource => CthluhuPath;

    public CthulhuOmen(Difficulty difficulty, ArkhamHorror arkhamHorror) : base(difficulty, arkhamHorror)
    {
        _deepOne = EldritchHorror.GetEldritchMinion(EldritchMinion.DeepOne).monsterDefinition;
        _starSpawn = EldritchHorror.GetEldritchMinion(EldritchMinion.StarSpawn).monsterDefinition;
        _maxDeepOnes = _deepOne.amount;
        _maxStarSpawns = _starSpawn.amount;
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

    protected override IEnumerator ActivateInterval(int symbols)
    {
        switch (symbols)
        {
            case > 6:
                break;
            case > 4:
                if (_starSpawns.Count < _maxStarSpawns)
                {
                    yield return SpawnMonsterGate(_starSpawn, Gate.Tesseract, _starSpawns);
                }
                var random = Random.Range(0, _starSpawns.Count - 1);
                yield return _starSpawns[random].Activate();
                break;
            case > 2:
                break;
            case > 0:
                foreach (var monster in _deepOnes)
                {
                    yield return monster.Activate();
                }
                for (var i = 0; i < 2 && i + _deepOnes.Count < _maxDeepOnes; i++)
                {
                    yield return SpawnMonsterGate(_deepOne, Gate.Trinity, _deepOnes);
                }
                break;
        }
    }

    private IEnumerator SpawnMonsterGate(MonsterDefinition monsterDefinition, Gate gate, List<Monster> list)
    {
        MonsterSpawn spawn = ArkhamHorror.MonsterSpawn(monsterDefinition, ArkhamHorror.GetGateBuilding(gate));
        yield return spawn;
        list.Add(spawn.Monster);
    }
}