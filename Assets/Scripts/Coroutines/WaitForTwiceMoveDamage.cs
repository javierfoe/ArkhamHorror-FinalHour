using System.Collections.Generic;

public class WaitForTwiceMoveDamage : WaitFor
{
    private readonly WaitForDamageMonsters _monsters;
    private readonly WaitForMoveUpTo _move;

    public Building MoveTo { get; private set; }
    public List<Monster> SelectedMonsters => _monsters.SelectedMonsters;

    public WaitForTwiceMoveDamage(int damage, Building building)
    {
        _monsters = new WaitForDamageMonsters(damage, building.GetAdjacentBuildings(), 1, building, damage * 2);
        _move = new WaitForMoveUpTo(building, 2);
    }


    public override bool MoveNext()
    {
        var moveBool = _move.MoveNext();
        var monsterBool = _monsters.MoveNext();

        if (!moveBool)
        {
            MoveTo = _move.MoveTo;
            ConfirmAction();
            return false;
        }

        if (monsterBool) return true;

        if (SelectedMonsters is { Count: > 0 })
            MoveTo = SelectedMonsters[0].Building;

        return false;
    }

    public override void ConfirmAction()
    {
        _monsters.ConfirmAction();
        base.ConfirmAction();
    }
}