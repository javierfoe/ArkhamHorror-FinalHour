using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArkhamHorror : MonoBehaviour
{
    public static readonly List<Monster> AliveMonsters = new();
    
    [SerializeField] private Transform monsters, buildings, investigators;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private Building targetBuilding;
    [SerializeField] private MonsterList baseMonsterList;
    [SerializeField] private int constantMonsterAmount;
    private Monster[] _monsters;
    private Building[] _buildings;
    private Investigator[] _investigators;

    private void Start()
    {
        /*
        _investigators = investigators.GetComponentsInChildren<Investigator>();
        foreach (var investigator in _investigators)
        {
            targetBuilding.MoveInvestigator(investigator);
        }

        _monsters = monsters.GetComponentsInChildren<Monster>();
        foreach (var monster in _monsters)
        {
            targetBuilding.IncomingMonster(monster);
        }
        */
        List<MonsterDefinition> startingMonsterPool = new();

        foreach (var monsterDef in baseMonsterList.monsterDefinitions)
        {
            for (var i = 0; i < monsterDef.amount; i++)
            {
                startingMonsterPool.Add(monsterDef);
            }
        }
        
        MonsterPool.SetStartingPool(startingMonsterPool);

        StartCoroutine(StartLoop());
    }

    private IEnumerator StartLoop()
    {
        yield return RespawnMonsters();

        _buildings = buildings.GetComponentsInChildren<Building>();
        foreach (var building in _buildings)
        {
            building.FinishMonsterMovement();
        }

        Debug.Log("Wait Start Loop");
        yield return null;
        
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
            
            Debug.Log("Finish round");
            yield return null;
        }
    }

    private IEnumerator SpawnMonster()
    {
        var monsterDefinition = MonsterPool.SpawnMonster();
        var monster = Instantiate(monsterPrefab, monsters).Initialize(monsterDefinition);
        Debug.Log("Monster Spawned", monster.gameObject);
        AliveMonsters.Add(monster);
        targetBuilding.IncomingMonster(monster);
        yield return null;
    }

    private IEnumerator RespawnMonsters()
    {
        var currentMonsters = AliveMonsters.Count;
        if (currentMonsters < constantMonsterAmount)
        {
            for (var i = 0; i < constantMonsterAmount - currentMonsters; i++)
            {
                yield return SpawnMonster();
            }
        }
    }
}