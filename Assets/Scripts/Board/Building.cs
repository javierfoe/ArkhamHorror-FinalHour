using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class Building : MonoBehaviour, IClickable<Building>
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

    public UnityEvent<Building> OnClick { get; } = new();

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
        set => gate = value;
    }

    public int GatePower { get; private set; }

    public void RemoveMonster(Monster monster)
    {
        if (_incomingMonsters.Contains(monster)) _incomingMonsters.Remove(monster);
        foreach (var monsters in _monsters.Values)
        {
            if (monsters.Contains(monster)) monsters.Remove(monster);
        }
    }

    public int GetDistanceTo(Building destination)
    {
        if (destination == this) return 0;
        var auxBuildings = new List<Building> { this };
        var distance = 1;
        var maxLoops = 6;
        while (distance < maxLoops)
        {
            var iteration = new List<Building>(auxBuildings);
            auxBuildings.Clear();
            foreach (var auxBuilding in iteration)
            {
                foreach (var adjacent in auxBuilding.GetAdjacentBuildings())
                {
                    if (adjacent == destination)
                    {
                        return distance;
                    }

                    if (auxBuildings.Contains(adjacent)) continue;
                    auxBuildings.Add(adjacent);
                }
            }

            distance++;
        }

        return -1;
    }

    public static IEnumerable<Building> GetDistanceBuildings(Building origin, int distance)
    {
        var result = new List<Building> { origin };
        var auxBuildings = new List<Building>(result);
        for (var i = 0; i < distance; i++)
        {
            var iteration = new List<Building>(auxBuildings);
            auxBuildings.Clear();
            foreach (var auxBuilding in iteration)
            {
                foreach (var adjacent in auxBuilding.GetAdjacentBuildings())
                {
                    if (result.Contains(adjacent)) continue;
                    result.Add(adjacent);
                    auxBuildings.Add(adjacent);
                }
            }
        }

        return result;
    }

    public int AddGate()
    {
        return ++GatePower;
    }

    public Pathway GetPathwayTo(Building building)
    {
        return !_pathways.ContainsKey(building) ? null : _pathways[building];
    }

    public IEnumerable<Pathway> GetPathways()
    {
        return _pathways.Values;
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

    public List<Investigator> GetInvestigators()
    {
        return new List<Investigator>(_investigators);
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

    public IEnumerable<Room> GetRepairableRooms()
    {
        var result = new List<Room>();
        foreach (var room in _rooms)
        {
            if (!room.Wreckage) continue;
            result.Add(room);
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

    public IEnumerator MoveInvestigator(Investigator investigator)
    {
        foreach (var location in _investigatorLocations)
        {
            if (!location.IsFree) continue;
            yield return investigator.SetLocation(location);
            break;
        }
    }

    public IEnumerator IncomingMonster(Monster monster)
    {
        var roomsAvailable = false;
        for (var i = 0; i < numberOfRooms && !roomsAvailable; i++)
        {
            var room = _rooms[i];
            if (!room.IsFree) continue;

            roomsAvailable = true;
            yield return monster.SetLocation(room);

            if (_incomingMonsters.Contains(monster)) continue;
            _incomingMonsters.Add(monster);
        }

        if (roomsAvailable) yield break;
        
        TraversedBuildings.Add(this);
        yield return MoveMonsterToNextBuilding(monster);
    }

    public IEnumerator MoveMonster(Monster monster)
    {
        TraversedBuildings.Clear();
        yield return MoveMonsterToNextBuilding(monster);
    }

    public IEnumerator Wreck(Monster monster)
    {
        monster.Room.Wreckage = true;
        yield return null;
        yield return monster.SetLocation(null);
        yield return IncomingMonster(monster);
    }

    public IEnumerator Kill()
    {
        foreach (var investigator in _investigators)
        {
            yield return investigator.Hit();
        }
    }

    public IEnumerator AddSeal(Pathway pathway, bool gray = false)
    {
        if (_pathways.ContainsValue(pathway))
        {
            yield return null;
        }
    }

    public void FinishMonsterMovement()
    {
        foreach (var monster in _incomingMonsters)
        {
            if (monster.Location && monster.Building != this) continue;
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

    private IEnumerator MoveMonsterToNextBuilding(Monster monster)
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
            yield return monster.Destroy();
        }
        else
        {
            yield return nextBuilding.IncomingMonster(monster);
        }
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

    public void OnMouseDown()
    {
        OnClick.Invoke(this);
    }
}