using System.Collections.Generic;

public class WaitForSelectMonsters : WaitForAllActions
{
    private readonly Building _startingBuilding;
    private readonly int _monsterAmount;
    private readonly bool _alwaysStartingBuilding, _adjacent, _monsterSelection;
    public List<Monster> DamagedMonsters => SelectedMonsters;

    public WaitForSelectMonsters(Building building, int distance = 1, int monsterAmount = 1) : base(building, false,
        false, false, true, distance)
    {
        _monsterAmount = monsterAmount;
        _monsterSelection = true;
    }

    public WaitForSelectMonsters(Building building, int distance, int damage, bool adjacent = false,
        bool alwaysStartingBuilding = false, bool seal = false) : base(building, distance, damage, seal)
    {
        _adjacent = adjacent;
        if (!alwaysStartingBuilding) return;
        _alwaysStartingBuilding = true;
        _startingBuilding = building;
    }

    protected override void UpdateMonstersCoroutine(Building building)
    {
        if(!_alwaysStartingBuilding) base.UpdateMonstersCoroutine(building);
    }

    protected override WaitForMonsterSelection ResetMonstersCoroutine(Building building)
    {
        if (_monsterSelection) return new WaitForMonsterSelection(new []{building}, _monsterAmount);
        if (!_adjacent) return base.ResetMonstersCoroutine(building);
        var auxBuilding = _alwaysStartingBuilding ? _startingBuilding : building;
        return new WaitForDamageMonsters(DamageAmount, auxBuilding.GetAdjacentBuildings(), 1);
    }
}