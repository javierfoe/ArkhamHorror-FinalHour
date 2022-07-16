using System.Collections.Generic;

public class WaitForDamageMonsters : WaitForMonsterSelection
{
    private readonly int _maxDamage;
    private Building _currentBuilding;
    private int _currentDamage;


    public WaitForDamageMonsters(int damage, IEnumerable<Building> buildings) : base(buildings)
    {
        _currentDamage = damage;
        _maxDamage = damage;
    }

    protected override void SelectMonster(Monster monster)
    {
        if (_currentBuilding != monster.Building)
        {
            _currentBuilding = monster.Building;
            _currentDamage = _maxDamage;
            ResetSelectedMonsters();
        }
        var hp = monster.MaxHp;
        if (IsSelected(monster))
        {
            _currentDamage += hp;
            RemoveSelectedMonster(monster);
        }
        else if (_currentDamage >= hp)
        {
            _currentDamage -= hp;
            AddSelectedMonster(monster);
        }
    }
}
