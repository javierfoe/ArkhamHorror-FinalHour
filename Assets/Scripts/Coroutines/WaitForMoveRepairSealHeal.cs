public class WaitForMoveRepairSealHeal : WaitForMoveOr
{
    private readonly Investigator _investigator;
    private readonly int _maxActions;
    private int _actions;
    private WaitForSelection<Pathway> _seal;
    private WaitForSelection<Room> _repair;
    private WaitForSelection<Investigator> _heal;

    public Pathway SealOn { get; private set; }
    public Room RepairOn { get; private set; }
    public bool Heal { get; private set; }

    public WaitForMoveRepairSealHeal(Investigator investigator, int distance = 1) : base(investigator.Building, distance)
    {
        _investigator = investigator;
        _maxActions = 2;
        Move.OnChangeBuilding.AddListener(UpdateBuilding);
        Move.OnRestart.AddListener(ResetCoroutines);
        ResetCoroutines();
    }

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
                RepairOn = _repair.SelectedElement;
                ConfirmAction();
                return false;
            }

            if (_actions < _maxActions)
            {
                _actions++;
                RepairOn = _repair.SelectedElement;
                ResetRepair(Default);
            }
        }

        if (!sealBool)
        {
            var currentSealOn = _seal.SelectedElement;
            if (currentSealOn == SealOn)
            {
                SealOn = _seal.SelectedElement;
                ConfirmAction();
                return false;
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
                return false;
            }

            if (_actions < _maxActions && !Heal)
            {
                _actions++;
                Heal = true;
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
        _actions++;
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
        Heal = false;
        ResetSeal(building);
        ResetRepair(building);
        ResetHeal();
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
}