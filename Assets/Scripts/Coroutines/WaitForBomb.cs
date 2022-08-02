using System.Collections.Generic;

public class WaitForBomb : WaitForMoveOr
{
    private readonly Investigator _investigator;
    private readonly int _damage, _distance;
    private WaitForDamageMonsters _damageCoroutine;
    private bool _bomb;
    public List<Monster> SelectedMonsters { get; private set; }
    private Building FirstBuilding => _investigator.Building;

    public WaitForBomb(Investigator investigator, int damage = 2, int distance = 2) : base(investigator.Building, distance)
    {
        _distance = distance;
        _damage = damage;
        _investigator = investigator;
        _investigator.OnClick.AddListener(SwitchBomb);
        ResetCoroutines();
    }

    public override bool MoveNext()
    {
        if (!_bomb)
        {
            var moveBool = _move.MoveNext();

            if (!moveBool)
            {
                MoveTo = _move.MoveTo;
                ConfirmAction();
                return false;
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
            ? new WaitForDamageMonsters(_damage, FirstBuilding.GetAdjacentBuildings(true))
            : new WaitForDamageMonsters(_damage, Building.GetDistanceBuildings(FirstBuilding, _distance), 1);
    }

    private void SwitchBomb(Investigator investigator)
    {
        _bomb = !_bomb;
        ResetCoroutines();
    }
}