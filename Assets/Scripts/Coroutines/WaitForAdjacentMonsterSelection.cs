public class WaitForAdjacentMonsterSelection : WaitForMonsterSelection
{
    private readonly int _maxMonsters;
    private int _currentMonsters;

    public WaitForAdjacentMonsterSelection(int amount, Building building) : base(building.GetAdjacentBuildings())
    {
        _maxMonsters = amount;
    }

    protected override void SelectMonster(Monster monster)
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
