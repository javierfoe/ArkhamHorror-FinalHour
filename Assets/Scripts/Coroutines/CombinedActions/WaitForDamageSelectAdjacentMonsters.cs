using System.Collections.Generic;

public class WaitForDamageSelectAdjacentMonsters : WaitForDamage
{
    private readonly int _monsterAmount;
    private WaitForMonsterSelection _adjacentMonsters;

    public List<Monster> AdditionalMonsters => _adjacentMonsters.SelectedMonsters;

    public WaitForDamageSelectAdjacentMonsters(Building building, int distance, int damage, int monsterAmount) :
        base(building, distance, damage)
    {
        _monsterAmount = monsterAmount;
    }

    protected override WaitForMonsterSelection ResetMonstersCoroutine(Building building)
    {
        _adjacentMonsters = new WaitForAdjacentMonsterSelection(_monsterAmount, building);
        return base.ResetMonstersCoroutine(building);
    }
}