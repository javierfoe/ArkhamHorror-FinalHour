using System.Collections.Generic;
using System.Linq;

public class WaitForDamageMonsters : WaitForMonsterSelection
{
    private readonly Dictionary<Building, List<Monster>> _buildingMonsters = new();
    private readonly Dictionary<Building, int> _buildingDamage = new();
    private readonly int _damage, _damageSpecial, _buildingAmount;
    private readonly Building _buildingSpecial;

    public WaitForDamageMonsters(int damage, IEnumerable<Building> buildings, int buildingAmount = 0,
        Building buildingSpecial = null, int damageSpecial = 0) : base(AddBuildingToSet(buildings, buildingSpecial))
    {
        _damage = damage;
        _damageSpecial = damageSpecial;
        _buildingSpecial = buildingSpecial;
        var length = buildings.Count() + (buildingSpecial ? 1 : 0);
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
        var damage = _damage;
        if (_buildingSpecial != null && building == _buildingSpecial)
        {
            damage = _damageSpecial;
        }

        _buildingDamage.Add(building, damage);
    }

    private void RemoveBuildingDictionary(Building building)
    {
        _buildingMonsters.Remove(building);
        _buildingDamage.Remove(building);
    }

    private static IEnumerable<Building> AddBuildingToSet(IEnumerable<Building> set, Building building)
    {
        var buildings = set.ToList();
        if (building != null)
        {
            buildings.Add(building);
        }

        return buildings;
    }
}