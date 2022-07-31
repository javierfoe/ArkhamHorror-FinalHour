using System.Collections.Generic;

public class WaitForTwiceMoveDamage : WaitFor
{
    private readonly Building _firstBuilding;
    private readonly WaitForDamageMonsters _monsters;
    private WaitForMoveUpTo _move;

    public Building MoveTo { get; private set; }
    public List<Monster> SelectedMonsters => _monsters.SelectedMonsters;

    public WaitForTwiceMoveDamage(int damage, Building building)
    {
        _firstBuilding = building;
        _monsters = new WaitForDamageMonsters(damage, building.GetAdjacentBuildings(), 1, building, damage * 2);
        ResetMove();
    }


    public override bool MoveNext()
    {
        var moveBool = _move.MoveNext();
        var monsterBool = _monsters.MoveNext();

        if (!moveBool)
        {
            var currentMoveTo = _move.SelectedElement;
            if (MoveTo != _firstBuilding && _firstBuilding == currentMoveTo)
            {
                MoveTo = currentMoveTo;
                Reset();
                return true;
            }

            if (currentMoveTo == MoveTo)
            {
                return false;
            }

            MoveTo = currentMoveTo;
            ResetMove();
        }

        if (monsterBool) return true;

        if (SelectedMonsters is { Count: > 0 })
            MoveTo = SelectedMonsters[0].Building;

        return false;
    }

    private void ResetMove()
    {
        _move = new WaitForMoveUpTo(_firstBuilding, 2);
    }

    public override void ConfirmAction()
    {
        _monsters.ConfirmAction();
        base.ConfirmAction();
    }
}