using System.Collections.Generic;

public class WaitForBomb : WaitFor
{
    private readonly Building _firstBuilding;
    private readonly Investigator _investigator;
    private readonly int _damage, _distance;
    private WaitForMoveUpTo _move;
    private WaitForDamageMonsters _damageCoroutine;
    private bool _bomb;
    public Building MoveTo { get; private set; }
    public List<Monster> SelectedMonsters { get; private set; }

    public WaitForBomb(Investigator investigator, int damage = 2, int distance = 2)
    {
        _distance = distance;
        _damage = damage;
        _investigator = investigator;
        _firstBuilding = investigator.Building;
        _investigator.OnClick.AddListener(SwitchBomb);
        ResetCoroutines();
    }

    public override bool MoveNext()
    {
        if (_move != null)
        {
            var moveBool = _move.MoveNext();

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
                    ConfirmAction();
                    return false;
                }

                MoveTo = currentMoveTo;
                ResetMove();
            }
        }

        if (_damageCoroutine.MoveNext()) return true;

        if (!_bomb && _damageCoroutine.SelectedMonsters is { Count: > 0 })
        {
            MoveTo = _damageCoroutine.SelectedMonsters[0].Building;
        }

        SelectedMonsters = _damageCoroutine.SelectedMonsters;

        return false;
    }

    public override void Reset()
    {
        base.Reset();
        ResetCoroutines();
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        _damageCoroutine.ConfirmAction();
        _investigator.OnClick.RemoveListener(SwitchBomb);
    }

    private void ResetCoroutines()
    {
        _damageCoroutine = _bomb
            ? new WaitForDamageMonsters(_damage, _investigator.Building.GetAdjacentBuildings(true))
            : new WaitForDamageMonsters(_damage, Building.GetDistanceBuildings(_investigator.Building, _distance), 1);
        ResetMove();
    }

    private void ResetMove()
    {
        _move = _bomb ? null : new WaitForMoveUpTo(_firstBuilding, _distance);
    }

    private void SwitchBomb(Investigator investigator)
    {
        _bomb = !_bomb;
        ResetCoroutines();
    }
}