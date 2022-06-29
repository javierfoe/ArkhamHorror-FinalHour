using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArkhamHorror : MonoBehaviour
{

    [Serializable]
    public class GateDefinition
    {
        public int tesseract, heptagram, trinity;
    }

    [SerializeField] private GateDefinition gateDefinition;
    [SerializeField] private Transform monsters, buildings, investigators;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private Building targetBuilding;
    [SerializeField] private Monsters baseMonsters;
    
    private readonly Dictionary<Gate, Building> _gateBuildings = new();
    private readonly DiscardList<MonsterDefinition> _monsterPool = new();
    private readonly DiscardList<Gate> _gates = new();
    private Building _ritualBuilding;
    private Monster[] _monsters;
    private Building[] _buildings;
    private Investigator[] _investigators;

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

        var random = Random.Range(0, 2);
        AddGate(gateDefinition.tesseract - (random == 0 ? 1 : 0), Gate.Tesseract);
        AddGate(gateDefinition.heptagram - (random == 1 ? 1 : 0), Gate.Heptagram);
        AddGate(gateDefinition.trinity - (random == 2 ? 1 : 0), Gate.Trinity);

        StartCoroutine(StartLoop());
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

        _buildings = buildings.GetComponentsInChildren<Building>();
        foreach (var building in _buildings)
        {
            building.FinishMonsterMovement();
            Gate gate = building.gate;
            switch (gate)
            {
                case Gate.Ritual:
                    _ritualBuilding = building;
                    break;
                case Gate.Heptagram:
                case Gate.Tesseract:
                case Gate.Trinity:
                    if (_gateBuildings.ContainsKey(gate))
                    {
                        Debug.LogWarning($"A building with a gate {gate} already exists: {_gateBuildings[gate]}. Ignoring this building", gameObject);
                        continue;
                    }
                    _gateBuildings.Add(gate, building);
                    AddGate(building);
                    break;
            }
        }
        
        yield return AddRandomGate();
        
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

    private IEnumerator SpawnMonster(Building building)
    {
        var monsterDefinition = _monsterPool.GetRandom();
        if (monsterDefinition == null) yield break;
        var monster = Instantiate(monsterPrefab, monsters).Initialize(monsterDefinition, MonsterDied);
        building.IncomingMonster(monster);
        yield return null;
    }

    private IEnumerator SpawnMonster()
    {
        yield return SpawnMonster(targetBuilding);
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