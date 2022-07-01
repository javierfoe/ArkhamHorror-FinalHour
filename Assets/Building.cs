using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Building : MonoBehaviour
{
    private const int MaxRooms = 4;
    private static readonly List<Building> TraversedBuildings = new();

    public Gate gate;
    [SerializeField] private int numberOfRooms = 4;
    [SerializeField] private Building redArrow, blueArrow;
    private readonly Dictionary<Building, Pathway> _pathways = new();
    private readonly List<Monster> _incomingMonsters = new(), _killers = new(), _stalkers = new(), _wreckers = new();
    private readonly List<Investigator> _investigators = new();
    private int _gatePower;
    private Room[] _rooms;
    private Location[] _investigatorLocations;

    public int AddGate()
    {
        return ++_gatePower;
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
        yield return ActivateMonsters(_killers);
        yield return ActivateMonsters(_stalkers);
        yield return ActivateMonsters(_wreckers);
    }

    private IEnumerator ActivateMonsters(List<Monster> monsters)
    {
        foreach (var monster in monsters)
        {
            yield return monster.Activate();
        }

        monsters.Clear();
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
            if (monster.Location && monster.Location.Building != this) continue;
            switch (monster.MainMonsterSkill)
            {
                case MonsterSkill.Killer:
                    _killers.Add(monster);
                    break;
                case MonsterSkill.Wrecker:
                    _wreckers.Add(monster);
                    break;
                case MonsterSkill.Stalker:
                    _stalkers.Add(monster);
                    break;
            }
        }

        _incomingMonsters.Clear();
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
        _rooms = transform.GetChild(0).GetComponentsInChildren<Room>();
        _investigatorLocations = transform.GetChild(1).GetComponentsInChildren<Location>();
        foreach (var location in _investigatorLocations)
        {
            location.Building = this;
        }

        for (var i = 0; i < numberOfRooms; i++)
        {
            _rooms[i].Building = this;
        }

        for (var i = numberOfRooms; i < MaxRooms; i++)
        {
            _rooms[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        var offset = new Vector3(0.25f, 0.25f, 0);
        Debug.DrawLine(transform.position + offset, redArrow.transform.position, UnityEngine.Color.red, 100);
        Debug.DrawLine(transform.position - offset, blueArrow.transform.position, UnityEngine.Color.blue, 100);
    }
}