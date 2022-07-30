using System.Collections.Generic;
using System.Linq;

public class WaitForDamageMonsters : WaitForMonsterSelection
{
    private readonly Dictionary<Building, List<Monster>> _buildingMonsters = new();
    private readonly Dictionary<Building, int> _buildingDamage = new();
    private readonly int _damage, _buildingAmount;

    public WaitForDamageMonsters(int damage, IEnumerable<Building> buildings, int buildingAmount = 0) : base(buildings)
    {
        _damage = damage;
        var length = buildings.Count();
        _buildingAmount = buildingAmount == 0 || buildingAmount > length ? length : buildingAmount;
    }

    protected override void SelectMonster(Monster monster)
    {
        var building = monster.Building;
        var hp = monster.MaxHp;

        if (!_buildingMonsters.ContainsKey(building) && _buildingMonsters.Count < _buildingAmount)
        {
            AddBuildingDictionary(building);
        }

        if (!_buildingMonsters.ContainsKey(building)) return;

        if (_buildingMonsters[building].Contains(monster))
        {
            _buildingDamage[building] += hp;
            _buildingMonsters[building].Remove(monster);
            RemoveSelectedMonster(monster);
            if (_buildingMonsters[building].Count < 1)
            {
                RemoveBuildingDictionary(building);
            }
        }
        else if (_buildingDamage[building] >= hp)
        {
            _buildingDamage[building] -= hp;
            _buildingMonsters[building].Add(monster);
            AddSelectedMonster(monster);
        }
    }

    private void AddBuildingDictionary(Building building)
    {
        _buildingMonsters.Add(building, new List<Monster>());
        _buildingDamage.Add(building, _damage);
    }

    private void RemoveBuildingDictionary(Building building)
    {
        _buildingMonsters.Remove(building);
        _buildingDamage.Remove(building);
    }
}