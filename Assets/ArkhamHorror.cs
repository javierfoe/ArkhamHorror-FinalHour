using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkhamHorror : MonoBehaviour
{
    
    [SerializeField] private Transform monsters, buildings, investigators;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private Building targetBuilding;
    [SerializeField] private MonsterList baseMonsterList;
    [SerializeField] private int constantMonsterAmount;
    
    private readonly Dictionary<Gate, Building> _gateBuildings = new();
    private readonly List<Monster> _aliveMonsters = new();
    private Building _ritualBuilding;
    private Monster[] _monsters;
    private Building[] _buildings;
    private Investigator[] _investigators;
    private MonsterPool _monsterPool;

    private void Start()
    {
        List<MonsterDefinition> startingMonsterPool = new();

        foreach (var monsterDef in baseMonsterList.monsterDefinitions)
        {
            for (var i = 0; i < monsterDef.amount; i++)
            {
                startingMonsterPool.Add(monsterDef);
            }
        }
        
        _monsterPool = new MonsterPool(startingMonsterPool);

        StartCoroutine(StartLoop());
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
        
        yield return RespawnMonsters();
        
        while (true)
        {
            foreach (var building in _buildings)
            {
                yield return building.ActivateMonsters();
            }
            
            yield return RespawnMonsters();

            foreach (var building in _buildings)
            {
                building.FinishMonsterMovement();
            }
            
            yield return null;
        }
    }

    private IEnumerator SpawnMonster(Building building)
    {
        var monsterDefinition = _monsterPool.SpawnMonster();
        if (monsterDefinition == null) yield break;
        var monster = Instantiate(monsterPrefab, monsters).Initialize(monsterDefinition, MonsterDied);
        _aliveMonsters.Add(monster);
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
        _aliveMonsters.Remove(monster);
        _monsterPool.MonsterDied(monster.MonsterDefinition);
    }

    private IEnumerator AddRandomGate()
    {
        var randomGate = (Gate)UnityEngine.Random.Range(0, 2);
        var building = _gateBuildings[randomGate];
        yield return AddGateAndSpawnMonster(building);
    }

    private IEnumerator KeepConstantMonsterNumber()
    {
        var currentMonsters = _aliveMonsters.Count;
        if (currentMonsters < constantMonsterAmount)
        {
            for (var i = 0; i < constantMonsterAmount - currentMonsters; i++)
            {
                yield return SpawnMonster();
            }
        }
    }

    private IEnumerator RespawnMonsters()
    {
        yield return AddRandomGate();
    }
}