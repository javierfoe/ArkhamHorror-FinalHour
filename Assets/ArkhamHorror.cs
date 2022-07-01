using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class ArkhamHorror : MonoBehaviour
{
    [Serializable]
    public class GatePool
    {
        public int tesseract, heptagram, trinity;
    }

    [SerializeField] private GatePool gatePool;
    [SerializeField] private Transform monsters, buildings, investigators;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private Building targetBuilding;
    [SerializeField] private Monsters baseMonsters;
    [SerializeField] private Difficulty difficulty;
    [SerializeField] private AncientOne eldritchHorror;
    [SerializeField] private EldritchHorror cthulhu, umordhoth, shuddemell;

    private readonly Dictionary<Gate, Building> _gateBuildings = new();
    private readonly Pool<MonsterDefinition> _monsterPool = new();
    private readonly Pool<Gate> _gates = new();
    private Monster[] _monsters;
    private Building[] _buildings;
    private Investigator[] _investigators;
    private AncientOneOmen _ancientOneOmen;

    private void Start()
    {
        List<MonsterDefinition> startingMonsterPool = new();

        foreach (var monsterDef in baseMonsters.monsterDefinitions)
        {
            for (var i = 0; i < monsterDef.amount; i++)
            {
                startingMonsterPool.Add(monsterDef);
            }
        }

        _monsterPool.AddRange(startingMonsterPool);

        var oneLessGate = Random.Range(0, 2);
        AddGate(gatePool.tesseract - (oneLessGate == 0 ? 1 : 0), Gate.Tesseract);
        AddGate(gatePool.heptagram - (oneLessGate == 1 ? 1 : 0), Gate.Heptagram);
        AddGate(gatePool.trinity - (oneLessGate == 2 ? 1 : 0), Gate.Trinity);

        _buildings = buildings.GetComponentsInChildren<Building>();

        foreach (var building in _buildings)
        {
            building.FinishMonsterMovement();
            Gate gate = building.gate;
            if (gate is Gate.None or Gate.Ritual) continue;
            if (_gateBuildings.ContainsKey(gate))
            {
                Debug.LogWarning(
                    $"A building with gate {gate} already exists: {_gateBuildings[gate]}. Ignoring this building",
                    gameObject);
                continue;
            }

            _gateBuildings.Add(gate, building);
            AddGate(building);
        }

        StartCoroutine(StartLoop());
    }

    public IEnumerator SelectEldritchHorrorDifficulty(AncientOne horror, Difficulty difficulty)
    {
        EldritchHorror selectedHorror;
        switch (horror)
        {
            case AncientOne.Umôrdhoth:
                selectedHorror = umordhoth;
                _ancientOneOmen = new UmordhothOmen(umordhoth.intervalMinimums);
                break;
            case AncientOne.ShuddeMell:
                selectedHorror = shuddemell;
                _ancientOneOmen = new ShuddeMellOmen(shuddemell.intervalMinimums);
                break;
            default:
                selectedHorror = cthulhu;
                _ancientOneOmen = new CthulhuOmen(cthulhu.intervalMinimums);
                break;
        }

        var difficultySetting = difficulty switch
        {
            Difficulty.Normal => selectedHorror.normal,
            Difficulty.Hard => selectedHorror.hard,
            _ => selectedHorror.easy
        };

        foreach (var building in _buildings)
        {
            if (building.gate != Gate.None) continue;
            yield return SpawnMonsters(difficultySetting.monstersOther, building);
        }

        foreach (var startingMonsters in difficultySetting.portals)
        {
            var gate = startingMonsters.gate;
            switch (gate)
            {
                case Gate.Heptagram:
                case Gate.Tesseract:
                case Gate.Trinity:
                    yield return SpawnStartingMonsters(_gateBuildings[gate], selectedHorror, startingMonsters);
                    break;
                case Gate.None:
                    foreach (var building in _gateBuildings.Values)
                    {
                        yield return SpawnStartingMonsters(building, selectedHorror, startingMonsters);
                    }

                    break;
            }
        }
    }

    private void AddGate(int amount, Gate gate)
    {
        for (var i = 0; i < amount; i++)
        {
            _gates.Add(gate);
        }
    }

    private IEnumerator StartLoop()
    {
        yield return SelectEldritchHorrorDifficulty(eldritchHorror, difficulty);

        while (true)
        {
            foreach (var building in _buildings)
            {
                yield return building.ActivateMonsters();
            }

            yield return AddRandomGate();

            foreach (var building in _buildings)
            {
                building.FinishMonsterMovement();
            }

            yield return null;
        }
    }

    private IEnumerator SpawnStartingMonsters(Building building, EldritchHorror eldritchHorror,
        StartingMonsters startingMonsters)
    {
        yield return SpawnMonsters(startingMonsters.standardMonsters, building);
        foreach (var eldritchMinion in startingMonsters.eldritchMinions)
        {
            var eldritchMinionDefinition = eldritchHorror.GetEldritchMinion(eldritchMinion.eldritchMinion);
            yield return SpawnMonsters(eldritchMinion.amount, eldritchMinionDefinition, building);
        }
    }

    private IEnumerator SpawnMonsters(int amount, MonsterDefinition monsterDefinition, Building building)
    {
        for (var i = 0; i < amount; i++)
        {
            yield return SpawnMonster(monsterDefinition, building);
        }
    }

    private IEnumerator SpawnMonsters(int amount, Building building)
    {
        for (var i = 0; i < amount; i++)
        {
            yield return SpawnMonster(building);
        }
    }

    private IEnumerator SpawnMonster(Building building)
    {
        var monsterDefinition = _monsterPool.GetRandom();
        if (monsterDefinition == null) yield break;
        yield return SpawnMonster(monsterDefinition, building);
    }

    private IEnumerator SpawnMonster(MonsterDefinition monsterDefinition, Building building)
    {
        var monster = Instantiate(monsterPrefab, monsters).Initialize(monsterDefinition, MonsterDied);
        building.IncomingMonster(monster);
        yield return null;
    }

    private int AddGate(Building building)
    {
        return building.AddGate();
    }

    private IEnumerator AddGateAndSpawnMonster(Building building)
    {
        var monstersToSpawn = AddGate(building);
        for (var i = 0; i < monstersToSpawn; i++)
        {
            yield return SpawnMonster(building);
        }
    }

    private void MonsterDied(Monster monster)
    {
        _monsterPool.Discard(monster.MonsterDefinition);
    }

    private IEnumerator AddRandomGate()
    {
        var randomGate = _gates.GetRandom();
        var building = _gateBuildings[randomGate];
        yield return AddGateAndSpawnMonster(building);
    }
}