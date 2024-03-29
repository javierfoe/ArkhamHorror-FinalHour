using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class ArkhamHorror : MonoBehaviour
{
    [Serializable]
    public class GatePool
    {
        public int tesseract, heptagram, trinity;
    }

    [SerializeField] private GoodAction goodAction;

    [SerializeField] private Button confirm;
    [SerializeField] private ActionCard actionCard;
    [SerializeField] private University university;
    [SerializeField] private GatePool gatePool;
    [SerializeField] private SpawnInvestigator investigatorPrefab;
    [SerializeField] private int investigatorAmount;
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

    public Investigator OtherInvestigator(Investigator investigator)
    {
        return _investigators[0] == investigator ? _investigators[1] : _investigators[0];
    }

    public MonsterSpawn MonsterSpawn(MonsterDefinition monsterDefinition, Building building)
    {
        return new MonsterSpawn(monsterPrefab, monsterDefinition, building, MonsterDied);
    }

    public Building GetGateBuilding(Gate gate)
    {
        return university.GetGateBuilding(gate);
    }

    public Building GetLowestGateBuilding()
    {
        return university.GetLowestGateBuilding();
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
        SetClues();
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

    public IEnumerator IncomingMonster(Monster monster, Gate gate)
    {
        yield return university.GetGateBuilding(gate).IncomingMonster(monster);
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

    public void SetActionCard(ActionCardDefinition actionDefinition)
    {
        actionCard.SetActionCard(actionDefinition);
    }

    public void SetBuildingGates(Buildings buildingSetup)
    {
        var buildings = university.Buildings;
        buildings[buildingSetup.heptagram].Gate = Gate.Heptagram;
        buildings[buildingSetup.trinity].Gate = Gate.Trinity;
        buildings[buildingSetup.ritual].Gate = Gate.Ritual;
        buildings[buildingSetup.tesseract].Gate = Gate.Tesseract;
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
        var monsterSpawn = new MonsterSpawn(monsterPrefab, monsterDefinition, building, MonsterDied);
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

        StartCoroutine(StartLoop());
    }

    private void SetClues()
    {
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
    }

    private IEnumerator SpawnInvestigators()
    {
        var buildings = university.Buildings;
        _investigators = new Investigator[investigatorAmount];
        for (var i = 0; i < investigatorAmount; i++)
        {
            var investigator = Instantiate(investigatorPrefab).Spawn(Character.JennyBarnes, 5);
            _investigators[i] = investigator;
            var building = buildings[Random.Range(0, buildings.Count - 1)];
            yield return building.MoveInvestigator(investigator);
        }
    }

    private IEnumerator StartLoop()
    {
        yield return SpawnInvestigators();

        yield return SelectEldritchHorrorDifficulty(eldritchHorror, difficulty);

        university.FinishMonsterMovement();
        while (true)
        {
            var investigator = _investigators[0];
            Debug.Log(goodAction);
            yield return StartGoodAction(investigator, goodAction);
        }
    }
}