using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private const int MaxRooms = 4;

    [SerializeField] private int numberOfRooms = 4;
    [SerializeField] private Building redArrow, blueArrow;
    private readonly Dictionary<Building, Pathway> _pathways = new();
    private readonly List<Monster> _incomingMonsters = new(), _killers = new(), _stalkers = new(), _wreckers = new();
    private readonly List<Investigator> _investigators = new();
    private Room[] _rooms;
    private Location[] _investigatorLocations;

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
        foreach (var monster in _killers)
        {
            Debug.Log("Killer monster", monster.gameObject);
            yield return monster.Activate();
        }
        foreach (var monster in _stalkers)
        {
            Debug.Log("Stalker monster", monster.gameObject);
            yield return monster.Activate();
        }
        foreach (var monster in _wreckers)
        {
            Debug.Log("Wrecker monster", monster.gameObject);
            yield return monster.Activate();
        }

        _killers.Clear();
        _stalkers.Clear();
        _wreckers.Clear();
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

    public void IncomingMonster(Monster monster, Building first = null)
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

        MoveMonsterToNextBuilding(monster, first);
    }

    public void MoveMonster(Monster monster)
    {
        MoveMonsterToNextBuilding(monster, this);
    }

    public void Wreck(Monster monster)
    {
        monster.Location.Wreckage = true;
        monster.Location = null;
        IncomingMonster(monster, this);
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
            switch (monster.MainSkill)
            {
                case Monster.Skill.Killer:
                    _killers.Add(monster);
                    break;
                case Monster.Skill.Wrecker:
                    _wreckers.Add(monster);
                    break;
                case Monster.Skill.Stalker:
                    _stalkers.Add(monster);
                    break;
            }
        }
        _incomingMonsters.Clear();
    }

    private void MoveMonsterToNextBuilding(Monster monster, Building first)
    {
        var nextBuilding = monster.Color switch
        {
            Color.Blue => blueArrow,
            Color.Red => redArrow,
            _ => throw new ArgumentOutOfRangeException()
        };

        var pathway = _pathways[nextBuilding];
        
        var deadMonster = pathway.MonsterDiesToSeal(monster);

        if (deadMonster || nextBuilding == first) return;

        nextBuilding.IncomingMonster(monster, first);
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
}