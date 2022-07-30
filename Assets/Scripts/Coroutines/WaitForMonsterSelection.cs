using System.Collections.Generic;

public abstract class WaitForMonsterSelection : WaitFor
{
    private readonly List<Monster> _selectedMonsters = new();
    private readonly IEnumerable<Monster> _monsters;
    public List<Monster> SelectedMonsters => new (_selectedMonsters);
    
    protected WaitForMonsterSelection(IEnumerable<Building> buildings)
    {
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
    }

    protected void RemoveSelectedMonster(Monster monster)
    {
        _selectedMonsters.Remove(monster);
    }

    protected abstract void SelectMonster(Monster monster);
}