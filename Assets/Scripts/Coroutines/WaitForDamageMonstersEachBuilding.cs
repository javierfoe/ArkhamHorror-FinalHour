using System.Collections.Generic;

public class WaitForDamageMonstersEachBuilding : WaitForMonsterSelection
{
    private readonly Dictionary<Building, List<Monster>> _buildingMonsters = new();
    private readonly Dictionary<Building, int> _buildingDamage = new();

    public WaitForDamageMonstersEachBuilding(int damage, IEnumerable<Building> buildings) : base(buildings)
    {
        foreach (var building in buildings)
        {
            _buildingMonsters.Add(building, new List<Monster>());
            _buildingDamage.Add(building, damage);
        }
    }

    protected override void SelectMonster(Monster monster)
    {
        var building = monster.Building;
        var hp = monster.MaxHp;
        if (_buildingMonsters[building].Contains(monster))
        {
            _buildingDamage[building] += hp;
            RemoveSelectedMonster(monster);
        } else if (_buildingDamage[building] >= hp)
        {
            _buildingDamage[building] -= hp;
            AddSelectedMonster(monster);
        }
    }
}
