public abstract class WaitForAllActions : WaitForMoveOr
{
    private readonly bool _sealBool, _repairBool, _healBool, _damageBool;
    private readonly int _maxActions;
    private int _actions;
    private WaitForSelection<Pathway> _seal;
    private WaitForSelection<Room> _repair;
    private WaitForSelection<Investigator> _heal;
    private WaitForDamageMonsters _damage;

    public Pathway SealOn { get; private set; }
    public Room RepairOn { get; private set; }
    public Investigator Heal { get; private set; }

    private WaitForAllActions(Investigator investigator, int distance) : base(investigator.Building, distance)
    {
    }

    protected WaitForAllActions(Investigator investigator, bool seal, bool repair, bool heal, bool damage,
        int maxActions = 5, int distance = 1) : this(investigator, distance)
    {
        _maxActions = maxActions;
        _sealBool = seal;
        _repairBool = repair;
        _healBool = heal;
        _damageBool = damage;
    }
/*
    public WaitForAllActions(Investigator investigator, int distance = 1) :
        this(investigator, true, true, true, false, 2, distance)
    {
        Move.OnChangeBuilding.AddListener(UpdateBuilding);
        Move.OnRestart.AddListener(ResetCoroutines);
        ResetCoroutines();
    }*/

    public override bool MoveNext()
    {
        var moveBool = Move.MoveNext();
        var sealBool = _seal.MoveNext();
        var repairBool = _repair.MoveNext();
        var healBool = _heal.MoveNext();

        if (!repairBool)
        {
            var currentRepairOn = _repair.SelectedElement;
            if (currentRepairOn == RepairOn)
            {
                RepairOn = currentRepairOn;
                ConfirmAction();
                return false;
            }

            if (_actions < _maxActions)
            {
                _actions++;
                RepairOn = currentRepairOn;
                ResetRepair(Default);
            }
        }

        if (!sealBool)
        {
            var currentSealOn = _seal.SelectedElement;
            if (currentSealOn == SealOn)
            {
                SealOn = currentSealOn;
                ConfirmAction();
                return false;
            }

            if (_actions < _maxActions)
            {
                _actions++;
                SealOn = currentSealOn;
                ResetSeal(Default);
            }
        }

        if (!healBool)
        {
            var currentHeal = _heal.SelectedElement;
            if (currentHeal == Heal)
            {
                Heal = currentHeal;
                ConfirmAction();
                return false;
            }

            if (_actions < _maxActions)
            {
                _actions++;
                Heal = currentHeal;
                ResetHeal();
            }
        }

        if (moveBool) return true;

        MoveTo = Default;
        ConfirmAction();
        return false;
    }

    public override void Reset()
    {
        base.Reset();
        ResetCoroutines();
    }

    private void UpdateBuilding(Building building)
    {
        if (_actions >= _maxActions) return;
        if (building != MoveTo && !IsOrigin(building))
        {
            _actions++;
        }

        MoveTo = building;

        _seal = new WaitForSelection<Pathway>(MoveTo.GetPathways());
        _repair = new WaitForSelection<Room>(MoveTo.GetRepairableRooms());
    }

    private void ResetCoroutines()
    {
        ResetCoroutines(Default);
    }

    private void ResetCoroutines(Building building)
    {
        _actions = 0;
        SealOn = null;
        RepairOn = null;
        Heal = null;
        ResetSeal(building);
        ResetRepair(building);
        ResetHeal();
    }

    protected virtual void ResetRepair(Building building)
    {
        _repair = new WaitForSelection<Room>(building.GetRepairableRooms());
    }

    protected virtual void ResetSeal(Building building)
    {
        _seal = new WaitForSelection<Pathway>(building.GetPathways());
    }

    protected virtual void ResetHeal()
    {
    }
}