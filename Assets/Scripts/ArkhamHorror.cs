using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ArkhamHorror : MonoBehaviour
{
    [Serializable]
    public class GatePool
    {
        public int tesseract, heptagram, trinity;
    }

    [SerializeField] private Button confirm, undo;
    [SerializeField] private ActionCard actionCard;
    [SerializeField] private University university;
    [SerializeField] private OmenCardContainer hand, actions;
    [SerializeField] private GatePool gatePool;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private Monsters baseMonsters;
    [SerializeField] private Difficulty difficulty;
    [SerializeField] private AncientOne eldritchHorror;

    private readonly Pool<MonsterDefinition> _monsterPool = new();
    private readonly Pool<OmenCardDefinition> _omenCards = new();
    private readonly Pool<Gate> _gates = new();
    private readonly Pool<Clue> _clues = new();
    private Ritual _ritual;
    private Investigator[] _investigators;
    private AncientOneOmen _ancientOneOmen;

    public MonsterSpawn MonsterSpawn(MonsterDefinition monsterDefinition)
    {
        return new MonsterSpawn(monsterPrefab, monsterDefinition, MonsterDied);
    }

    public IEnumerator SelectEldritchHorrorDifficulty(AncientOne horror, Difficulty difficulty)
    {
        switch (horror)
        {
            case AncientOne.Umôrdhoth:
                _ancientOneOmen = new UmordhothOmen(difficulty, this);
                break;
            case AncientOne.ShuddeMell:
                _ancientOneOmen = new ShuddeMellOmen(difficulty, this);
                break;
            default:
                _ancientOneOmen = new CthulhuOmen(difficulty, this);
                break;
        }

        yield return _ancientOneOmen.SpawnStartingMonsters();
    }

    public IEnumerator SpawnMonstersOtherBuildings(int amount)
    {
        var buildings = university.GetOtherBuildings();
        foreach (var building in buildings)
        {
            for (var i = 0; i < amount; i++)
            {
                yield return SpawnMonster(building);
            }
        }
    }

    public IEnumerator SpawnMonstersGate(int amount, Gate gate)
    {
        var buildings = university.GetGateBuildings(gate);
        foreach (var building in buildings)
        {
            yield return SpawnMonsters(amount, building);
        }
    }

    public void RemoveRandomSeal()
    {
        var seals = university.GetSeals();
        var random = Random.Range(0, seals.Count - 1);
        seals[random].Disable();
    }

    public IEnumerator DamageRitual(int amount)
    {
        yield return university.DamageRitual(amount);
    }

    public void IncomingMonsterLowestGate(Monster monster)
    {
        university.GetLowestGateBuilding().IncomingMonster(monster);
    }

    public void IncomingMonster(Monster monster, Gate gate)
    {
        university.GetGateBuilding(gate).IncomingMonster(monster);
    }

    public IEnumerator SpawnMonsters(int amount, Building building)
    {
        for (var i = 0; i < amount; i++)
        {
            yield return SpawnMonster(building);
        }
    }

    public EldritchMinionsSpawn SpawnEldritchMinionsGate(EldritchMinionDefinition[] eldritchMinionDefinitions,
        Gate gate)
    {
        var buildings = university.GetGateBuildings(gate);

        EldritchMinionsSpawn eldritchMinionsSpawn =
            new EldritchMinionsSpawn(eldritchMinionDefinitions, buildings, this);

        return eldritchMinionsSpawn;
    }

    public void SetActionCard(ActionDefinition actionDefinition)
    {
        actionCard.SetActionCard(actionDefinition);
    }

    private void AddGate(int amount, Gate gate)
    {
        for (var i = 0; i < amount; i++)
        {
            _gates.Add(gate);
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
        var monsterSpawn = new MonsterSpawn(monsterPrefab, monsterDefinition, MonsterDied);
        building.IncomingMonster(monsterSpawn.Monster);
        yield return monsterSpawn;
    }

    private int AddGate(Building building)
    {
        return building.AddGate();
    }

    private IEnumerator AddGateAndSpawnMonster(Gate gate)
    {
        var building = university.GetGateBuilding(gate);
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
        yield return AddGateAndSpawnMonster(randomGate);
    }

    private void Awake()
    {
        ClueSprites.Initialize();
    }

    private void Start()
    {
        OmenSymbols omenSymbols = Resources.Load("OmenSymbols") as OmenSymbols;

        for (var i = 0; i < omenSymbols.cards.Length; i++)
        {
            var omenCard = new OmenCardDefinition();
            omenCard.Number = i + 1;
            omenCard.Clue = (Clue)(i % 5);
            omenCard.Omens = omenSymbols.cards[i];
            _omenCards.Add(omenCard);
        }

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

        for (var i = 0; i < 5; i++)
        {
            var clue = (Clue)(i % 5);
            _clues.Add(clue);
            _clues.Add(clue);
        }

        var firstRitualSymbol = _clues.GetRandom();
        var secondRitualSymbol = _clues.GetRandom();

        _ritual = new Ritual(firstRitualSymbol, secondRitualSymbol);

        for (var i = 0; i < 3; i++)
        {
            _clues.Add(Clue.Key);
        }

        var buildings = university.GetOtherBuildings();
        foreach (var building in buildings)
        {
            if (building.Gate != Gate.None) continue;
            building.Clue = _clues.GetRandom();
        }

        StartCoroutine(StartLoop());
    }

    private IEnumerator StartLoop()
    {
        yield return SelectEldritchHorrorDifficulty(eldritchHorror, difficulty);
        
        university.FinishMonsterMovement();
        while (true)
        {
            var waitFor = new WaitForMovementPathway(4, university.GetGateBuilding(Gate.Heptagram));
            
            yield return waitFor;
            /*
            Debug.Log(waitFor.Pathways.Count);
            
            foreach (var pathway in waitFor.Pathways)
            {
                Debug.Log($"Pathway {pathway.gameObject}", pathway.gameObject);
            }*/

            Debug.Log(waitFor.Buildings.Count);
            foreach (var building in waitFor.Buildings)
            {
                Debug.Log($"Building {building.gameObject}", building.gameObject);
            }
        }
    }
}