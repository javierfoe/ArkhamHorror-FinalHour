public class WaitForAdjacentMonsterSelection : WaitForMonsterSelection
{
    public WaitForAdjacentMonsterSelection(int amount, Building building) : base(building.GetAdjacentBuildings(), amount)
    {
    }
}