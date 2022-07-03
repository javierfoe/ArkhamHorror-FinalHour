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

    [SerializeField] private OmenCardContainer hand, actions;
    [SerializeField] private GatePool gatePool;
    [SerializeField] private Transform buildings, investigators;
    [SerializeField] private Monster monsterPrefab;
    [SerializeField] private Monsters baseMonsters;
    [SerializeField] private Difficulty difficulty;
    [SerializeField] private AncientOne eldritchHorror;

    private readonly Dictionary<Gate, Building> _gateBuildings = new();
    private readonly Pool<MonsterDefinition> _monsterPool = new();
    private readonly Pool<OmenCardDefinition> _omenCards = new();
    private readonly Pool<Gate> _gates = new();
    private Building _ritual;
    private Monster[] _monsters;
    private Building[] _buildings;
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
        foreach (var building in _buildings)
        {
            if (building.Gate != Gate.None) continue;
            for (var i = 0; i < amount; i++)
            {
                yield return SpawnMonster(building);
            }
        }
    }

    public IEnumerator SpawnMonstersGate(int amount, Gate gate)
    {
        foreach (var building in _buildings)
        {
            if (gate == Gate.None && building.Gate is Gate.None or Gate.Ritual
                || gate != Gate.None && building.Gate != gate) continue;
            yield return SpawnMonsters(amount, building);
        }
    }

    public IEnumerator RemoveRandomSeal()
    {
        List<Seal> seals = new();
        foreach (var building in _buildings)
        {
            foreach (var seal in building.GetActiveSeals())
            {
                if (seals.Contains(seal)) continue;
                seals.Add(seal);
            }
        }

        var random = Random.Range(0, seals.Count - 1);
        yield return seals[random].Disable();
    }

    public IEnumerator DamageRitual(int amount)
    {
        yield return _ritual.Wreck(amount);
    }

    public void IncomingMonsterLowestGate(Monster monster)
    {
        var aux = 10;
        Building lowestGatePower = null;
        foreach (var building in _buildings)
        {
            if (building.Gate is Gate.None or Gate.Ritual) continue;
            var gatePower = building.GatePower;
            if (gatePower >= aux) continue;
            lowestGatePower = building;
            aux = gatePower;
        }

        if (lowestGatePower != null)
        {
            lowestGatePower.IncomingMonster(monster);
        }
    }

    public void IncomingMonster(Monster monster, Gate gate)
    {
        foreach (var building in _buildings)
        {
            if (gate != building.Gate) continue;
            building.IncomingMonster(monster);
        }
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
        Building[] buildings = new Building[gate != Gate.None ? 1 : 3];

        var i = 0;
        foreach (var building in _buildings)
        {
            if (gate != Gate.None && building.Gate == gate ||
                gate == Gate.None && building.Gate is not (Gate.None or Gate.Ritual))
            {
                buildings[i++] = building;
            }
        }

        EldritchMinionsSpawn eldritchMinionsSpawn =
            new EldritchMinionsSpawn(eldritchMinionDefinitions, buildings, this);

        return eldritchMinionsSpawn;
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
        while (true)
        {
            hand.SetOmenCards(_omenCards.GetRandom(5));
            var waitFor = hand.WaitForCardSelection();
            yield return waitFor;
            _omenCards.Discard(waitFor.SelectedCard);
            _omenCards.Discard(waitFor.DiscardedCards);
            actions.SetOmenCards(waitFor.DiscardedCards);
        }
        /*
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
        }*/
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

    private void Awake()
    {
        OmenSprites.Initialize();
    }

    private void Start()
    {
        OmenSymbols omenSymbols = Resources.Load("OmenSymbols") as OmenSymbols;

        for (var i = 0; i < omenSymbols.cards.Length; i++)
        {
            var omenCard = new OmenCardDefinition();
            omenCard.number = i;
            omenCard.clue = (Clue)(i % 5);
            omenCard.omens = omenSymbols.cards[i];
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

        _buildings = buildings.GetComponentsInChildren<Building>();

        foreach (var building in _buildings)
        {
            building.FinishMonsterMovement();
            Gate gate = building.Gate;
            switch (gate)
            {
                case Gate.None:
                    break;
                case Gate.Ritual:
                    _ritual = building;
                    break;
                default:
                    if (_gateBuildings.ContainsKey(gate))
                    {
                        Debug.LogWarning(
                            $"A building with gate {gate} already exists: {_gateBuildings[gate]}. Ignoring this building",
                            gameObject);
                        continue;
                    }

                    _gateBuildings.Add(gate, building);
                    AddGate(building);
                    break;
            }
        }

        StartCoroutine(StartLoop());
    }
}