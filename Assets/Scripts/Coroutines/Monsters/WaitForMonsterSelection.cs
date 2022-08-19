using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitForMonsterSelection : WaitFor
{
    public readonly UnityEvent OnNotEmpty = new(), OnEmptied = new();

    private readonly List<Monster> _selectedMonsters = new();
    private readonly IEnumerable<Monster> _monsters;
    private readonly int _maxMonsters;
    private int _currentMonsters;

    private bool _empty = true;
    public List<Monster> SelectedMonsters => new(_selectedMonsters);

    public WaitForMonsterSelection(IEnumerable<Building> buildings, int amount = int.MaxValue)
    {
        _maxMonsters = amount;
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

    protected bool IsSelected(Monster monster)
    {
        return _selectedMonsters.Contains(monster);
    }

    protected void AddSelectedMonster(Monster monster)
    {
        _selectedMonsters.Add(monster);
        if (_empty)
        {
            OnNotEmpty.Invoke();
        }

        _empty = false;
    }

    protected void RemoveSelectedMonster(Monster monster)
    {
        _selectedMonsters.Remove(monster);
        if (_selectedMonsters.Count > 0) return;
        _empty = true;
        OnEmptied.Invoke();
    }


    protected virtual void SelectMonster(Monster monster)
    {
        if (IsSelected(monster))
        {
            _currentMonsters--;
            RemoveSelectedMonster(monster);
        } else if (_currentMonsters < _maxMonsters)
        {
            _currentMonsters++;
            AddSelectedMonster(monster);
        }
    }
}