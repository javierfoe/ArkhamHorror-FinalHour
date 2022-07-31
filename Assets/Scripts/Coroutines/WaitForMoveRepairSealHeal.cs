using TMPro;

public class WaitForMoveRepairSealHeal : WaitFor
{
    private readonly Building _firstBuilding;
    private readonly Investigator _investigator;
    private readonly int _distance;
    private readonly int _maxActions;
    private int _actions;
    private WaitForMoveUpTo _move;
    private WaitForSelection<Pathway> _seal;
    private WaitForSelection<Room> _repair;
    private WaitForSelection<Investigator> _heal;

    private Building Default => MoveTo ? MoveTo : _firstBuilding;
    public Building MoveTo { get; private set; }
    public Pathway SealOn { get; private set; }
    public Room RepairOn { get; private set; }
    public bool Heal { get; private set; }
    public bool FirstMove { get; private set; }

    public WaitForMoveRepairSealHeal(Investigator investigator, int distance = 1)
    {
        _investigator = investigator;
        _maxActions = 2;
        _distance = distance;
        _firstBuilding = investigator.Building;
        ResetCoroutines();
    }

    public override bool MoveNext()
    {
        if (!base.MoveNext()) return false;

        var moveBool = _move.MoveNext();
        var sealBool = _seal.MoveNext();
        var repairBool = _repair.MoveNext();
        var healBool = _heal.MoveNext();

        if (!repairBool)
        {
            var currentRepairOn = _repair.SelectedElement;
            if (currentRepairOn == RepairOn)
            {
                RepairOn = _repair.SelectedElement;
                MoveTo = Default;
                ConfirmAction();
                return true;
            }

            if (_actions < _maxActions)
            {
                _actions++;
                RepairOn = _repair.SelectedElement;
                ResetMove();
                ResetRepair(Default);
                return true;
            }
        }

        if (!sealBool)
        {
            var currentSealOn = _seal.SelectedElement;
            if (currentSealOn == SealOn)
            {
                SealOn = _seal.SelectedElement;
                MoveTo = Default;
                ConfirmAction();
                return true;
            }

            if (_actions < _maxActions)
            {
                _actions++;
                SealOn = _seal.SelectedElement;
                ResetSeal(Default);
            }
        }

        if (!healBool)
        {
            if (Heal)
            {
                ConfirmAction();
                return true;
            }

            if (_actions < _maxActions && !Heal)
            {
                _actions++;
                Heal = true;
                ResetHeal();
            }
        }

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
                return true;
            }

            if (_actions < _maxActions)
            {
                _actions++;
                MoveTo = currentMoveTo;
                ResetMove();
            }
        }

        if (!moveBool && sealBool && healBool && repairBool && !FirstMove && MoveTo != _firstBuilding)
        {
            _seal = new WaitForSelection<Pathway>(MoveTo.GetPathways());
            _repair = new WaitForSelection<Room>(MoveTo.GetRepairableRooms());
            FirstMove = true;
        }

        return true;
    }

    public override void Reset()
    {
        base.Reset();
        ResetCoroutines();
    }

    private void ResetCoroutines()
    {
        _actions = 0;
        FirstMove = false;
        SealOn = null;
        RepairOn = null;
        ResetMove();
        ResetSeal(_firstBuilding);
        ResetRepair(_firstBuilding);
        ResetHeal();
        Heal = false;
    }

    private void ResetRepair(Building building)
    {
        _repair = new WaitForSelection<Room>(building.GetRepairableRooms());
    }

    private void ResetSeal(Building building)
    {
        _seal = new WaitForSelection<Pathway>(building.GetPathways());
    }

    private void ResetHeal()
    {
        _heal = new WaitForSelection<Investigator>(_investigator.FullHp ? null : new[] { _investigator });
    }

    private void ResetMove()
    {
        _move = new WaitForMoveUpTo(_firstBuilding, _distance);
    }
}