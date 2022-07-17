using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Building : Clickable<Building>
{
    private static readonly List<Building> TraversedBuildings = new();

    [SerializeField] private Zone zone;
    [SerializeField] private ClueSymbol clue;
    [SerializeField] private Transform rooms, playerSpots;
    [SerializeField] private Gate gate;
    [SerializeField] private int numberOfRooms = 4;
    [SerializeField] private Building redArrow, blueArrow;
    private readonly Dictionary<Building, Pathway> _pathways = new();
    private readonly Dictionary<MonsterSkill, List<Monster>> _monsters = new();
    private readonly List<Monster> _incomingMonsters = new();
    private readonly List<Investigator> _investigators = new();
    private Room[] _rooms;
    private Location[] _investigatorLocations;

    public Zone Zone => zone;

    public Clue Clue
    {
        get => clue.Clue;
        set
        {
            clue.gameObject.SetActive(true);
            clue.Clue = value;
        }
    }

    public Gate Gate
    {
        get => gate;
        private set => gate = value;
    }

    public int GatePower { get; private set; }

    public int AddGate()
    {
        return ++GatePower;
    }

    public Pathway GetPathwayTo(Building building)
    {
        return !_pathways.ContainsKey(building) ? null : _pathways[building];
    }

    public List<Building> GetAdjacentBuildings(bool includeSelf = false, bool withoutPathway = false)
    {
        var result = _pathways.Keys.ToList();
        if (withoutPathway)
        {
            if (result.Contains(redArrow))
            {
                result.Add(redArrow);
            }
            if (!result.Contains(blueArrow))
            {
                result.Add(blueArrow);
            }
        }
        if (includeSelf)
        {
            result.Add(this);
        }
        return result;
    }

    public List<Monster> GetMonsters()
    {
        var result = new List<Monster>();
        foreach (var monsters in _monsters.Values)
        {
            result.AddRange(monsters);
        }

        return result;
    }

    public List<Seal> GetActiveSeals()
    {
        var result = new List<Seal>();
        foreach (var pathway in _pathways.Values)
        {
            var seal = pathway.Seal;
            if (!seal.Enabled) continue;
            result.Add(seal);
        }

        return result;
    }

    public IEnumerator Wreck(int amount)
    {
        for (int i = 0, damage = 0; i < _rooms.Length && damage < amount; i++)
        {
            var room = _rooms[i];
            if (!room.IsFree) continue;
            damage++;
            room.Wreckage = true;
            yield return null;
        }
    }

    public void AddInvestigator(Investigator investigator)
    {
        if (_investigators.Contains(investigator)) return;
        _investigators.Add(investigator);
    }

    public void RemoveInvestigator(Investigator investigator)
    {
        if (!_investigators.Contains(investigator)) return;
        _investigators.Remove(investigator);
    }

    public void AddAdjacent(Building building, Pathway pathway)
    {
        if (_pathways.ContainsKey(building))
        {
            Debug.LogError($"{gameObject.name} already contains {building.gameObject.name} as an adjacent Building",
                gameObject);
            return;
        }

        _pathways.Add(building, pathway);
    }

    public IEnumerator ActivateMonsters()
    {
        yield return ActivateMonsters(_monsters[MonsterSkill.Killer]);
        yield return ActivateMonsters(_monsters[MonsterSkill.Stalker]);
        yield return ActivateMonsters(_monsters[MonsterSkill.Wrecker]);
    }

    public void MoveInvestigator(Investigator investigator)
    {
        foreach (var location in _investigatorLocations)
        {
            if (!location.IsFree) continue;
            investigator.Location = location;
            break;
        }
    }

    public void IncomingMonster(Monster monster)
    {
        var roomsAvailable = false;
        for (var i = 0; i < numberOfRooms && !roomsAvailable; i++)
        {
            var room = _rooms[i];
            if (!room.IsFree) continue;

            roomsAvailable = true;
            monster.Location = room;

            if (_incomingMonsters.Contains(monster)) continue;
            _incomingMonsters.Add(monster);
        }

        if (roomsAvailable) return;

        TraversedBuildings.Add(this);

        MoveMonsterToNextBuilding(monster);
    }

    public void MoveMonster(Monster monster)
    {
        TraversedBuildings.Clear();
        MoveMonsterToNextBuilding(monster);
    }

    public void Wreck(Monster monster)
    {
        monster.Location.Wreckage = true;
        monster.Location = null;
        IncomingMonster(monster);
    }

    public void Kill()
    {
        foreach (var investigator in _investigators)
        {
            investigator.Hit(1);
        }
    }

    public void FinishMonsterMovement()
    {
        foreach (var monster in _incomingMonsters)
        {
            if (monster.Location && monster.Building!= this) continue;
            _monsters[monster.MainMonsterSkill].Add(monster);
        }

        _incomingMonsters.Clear();
    }

    private IEnumerator ActivateMonsters(List<Monster> monsters)
    {
        foreach (var monster in monsters)
        {
            yield return monster.Activate();
        }

        monsters.Clear();
    }

    private void MoveMonsterToNextBuilding(Monster monster)
    {
        var nextBuilding = monster.Color switch
        {
            Color.Blue => blueArrow,
            Color.Red => redArrow,
            _ => throw new ArgumentOutOfRangeException()
        };

        var deadMonster = false;

        if (_pathways.Count > 0 && _pathways.ContainsKey(nextBuilding))
        {
            var pathway = _pathways[nextBuilding];
            deadMonster = pathway.MonsterDiesToSeal(monster);
        }

        if (deadMonster || TraversedBuildings.Contains(nextBuilding))
        {
            monster.Destroy();
            return;
        }

        nextBuilding.IncomingMonster(monster);
    }

    private void Awake()
    {
        _rooms = rooms.GetComponentsInChildren<Room>();
        _investigatorLocations = playerSpots.GetComponentsInChildren<Location>();
        Gate = gate;
        foreach (var location in _investigatorLocations)
        {
            location.Building = this;
        }

        for (var i = 0; i < numberOfRooms; i++)
        {
            _rooms[i].Building = this;
        }

        clue.gameObject.SetActive(false);

        _monsters.Add(MonsterSkill.None, new List<Monster>());
        _monsters.Add(MonsterSkill.Killer, new List<Monster>());
        _monsters.Add(MonsterSkill.Wrecker, new List<Monster>());
        _monsters.Add(MonsterSkill.Stalker, new List<Monster>());
    }

    private void Start()
    {
        var offset = new Vector3(0.25f, 0.25f, 0);
        Debug.DrawLine(transform.position + offset, redArrow.transform.position, UnityEngine.Color.red, 100);
        Debug.DrawLine(transform.position - offset, blueArrow.transform.position, UnityEngine.Color.blue, 100);
    }

    protected override Building InvokeArgument()
    {
        return this;
    }
}