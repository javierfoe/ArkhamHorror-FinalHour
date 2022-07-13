using System.Collections.Generic;
using UnityEngine;

public class WaitForDamageMonsters : WaitFor
{
    private readonly IEnumerable<Monster> _monsters;
    private readonly int _maxDamage;
    private Building _currentBuilding;
    private int _currentDamage;

    public readonly List<Monster> DeadMonsters = new();

    public WaitForDamageMonsters(int damage, IEnumerable<Building> buildings)
    {
        _currentDamage = damage;
        _maxDamage = damage;
        List<Monster> monsters = new();
        foreach (var building in buildings)
        {
            monsters.AddRange(building.GetMonsters());
        }
        _monsters = monsters;
        foreach (var monster in _monsters)
        {
            monster.OnClick.AddListener(SelectMonster);
        }
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        foreach (var monster in _monsters)
        {
            monster.OnClick.RemoveListener(SelectMonster);
        }
    }

    private void SelectMonster(Monster monster)
    {
        if (_currentBuilding != monster.Building)
        {
            _currentBuilding = monster.Building;
            _currentDamage = _maxDamage;
            DeadMonsters.Clear();
        }
        var hp = monster.MaxHp;
        if (DeadMonsters.Contains(monster))
        {
            _currentDamage += hp;
            DeadMonsters.Remove(monster);
        }
        else if (_currentDamage >= hp)
        {
            _currentDamage -= hp;
            DeadMonsters.Add(monster);
        }
    }
}
