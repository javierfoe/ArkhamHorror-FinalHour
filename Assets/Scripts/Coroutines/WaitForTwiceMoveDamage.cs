using System.Collections.Generic;

public class WaitForTwiceMoveDamage : WaitFor
{
    private readonly int _damage, _distance;
    private readonly Building _origin;
    private readonly IEnumerable<Building> _buildings;
    private WaitForDoubleClickBuilding _move;
    private WaitForDamageMonsters _monsters;

    public Building MoveTo { get; private set; }
    public List<Monster> SelectedMonsters => _monsters.SelectedMonsters;

    public WaitForTwiceMoveDamage(int damage, Building origin, IEnumerable<Building> buildings, int distance = 1)
    {
        _distance = distance;
        _buildings = buildings;
        _origin = origin;
        _damage = damage;
        ResetCoroutines();
    }


    public override bool MoveNext()
    {
        var moveBool = _move.MoveNext();
        var monsterBool = _monsters.MoveNext();

        if (!moveBool || !monsterBool)
        {
            ConfirmAction();
            return false;
        }
        
        ResetMove();
        return true;
    }

    public override void ConfirmAction()
    {
        MoveTo = _move.MoveTo;
        _monsters.ConfirmAction();
        base.ConfirmAction();
    }

    private void ResetMove()
    {
        var distance = _monsters == null || _monsters.TotalDamage == 0 ? _distance * 2 : (_monsters.TotalDamage > 3 ? 0 : _distance);
        if (_move != null && _origin.GetDistanceTo(_move.MoveTo) <= distance) return;
        _move = new WaitForDoubleClickBuilding(_origin, distance);
        _move.OnChangeBuilding.AddListener(ResetDamage);
        _move.OnRestart.AddListener(ResetDamageZero);
    }

    private void ResetDamageZero(Building building)
    {
        ResetDamageZero(true);
    }

    private void ResetDamageZero(bool twice)
    {
        _monsters = new WaitForDamageMonsters(_damage, _buildings, twice);
    }

    private void ResetDamage(Building building)
    {
        var distance = _origin.GetDistanceTo(building);
        var damage = distance > 1 ? 0 : distance > 0 ? _damage : _damage * 2;
        if (_monsters != null && _monsters.TotalDamage <= damage) return;
        var twice = distance == 0;
        ResetDamageZero(twice);
    }

    private void ResetDamage()
    {
        ResetDamage(_move.MoveTo);
    }

    private void ResetCoroutines()
    {
        ResetMove();
        ResetDamage();
    }
}