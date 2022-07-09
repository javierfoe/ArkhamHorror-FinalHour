using System.Collections;
using System.Collections.Generic;

public class EldritchMinionsSpawn : IEnumerator
{
    public readonly Dictionary<EldritchMinion, List<Monster>> Monsters = new();
    private readonly ArkhamHorror _arkhamHorror;
    private readonly EldritchMinionDefinition[] _eldritchMinions;
    private readonly Building[] _buildings;
    private int _currentMonster, _currentMonsterAmount, _currentBuilding, _currentMonsterMax;

    public object Current { get; private set; }

    public EldritchMinionsSpawn(EldritchMinionDefinition[] eldritchMinions, Building[] buildings,
        ArkhamHorror arkhamHorror)
    {
        _buildings = buildings;
        _arkhamHorror = arkhamHorror;
        _eldritchMinions = eldritchMinions;
        _currentMonsterMax = eldritchMinions[0].monsterDefinition.amount;
    }

    public bool MoveNext()
    {
        if (_currentMonsterAmount == _currentMonsterMax)
        {
            _currentMonster++;
            _currentMonsterAmount = 0;
        }

        if (_currentMonster == _eldritchMinions.Length)
        {
            _currentBuilding++;
            _currentMonster = 0;
        }

        if (_currentBuilding == _buildings.Length)
        {
            return false;
        }

        _currentMonsterAmount++;
        var eldritchMinionDefinition = _eldritchMinions[_currentMonster];
        _currentMonsterMax = eldritchMinionDefinition.monsterDefinition.amount;
        var eldritchMinion = eldritchMinionDefinition.eldritchMinion;
        var monsterSpawn = _arkhamHorror.MonsterSpawn(eldritchMinionDefinition.monsterDefinition);
        if (!Monsters.ContainsKey(eldritchMinionDefinition.eldritchMinion))
        {
            Monsters.Add(eldritchMinionDefinition.eldritchMinion, new List<Monster>());
        }

        Monsters[eldritchMinion].Add(monsterSpawn.Monster);
        _buildings[_currentBuilding].IncomingMonster(monsterSpawn.Monster);
        Current = monsterSpawn;
        return true;
    }

    public void Reset()
    {
    }
}