public class WaitForMoveSeal : WaitForMoveOr
{
    private WaitForSelection<Pathway> _seal;
    public Pathway SealOn { get; private set; }

    public WaitForMoveSeal(Building building, int distance = 1) : base(building, distance)
    {
        Move.OnChangeBuilding.AddListener(ResetSeal);
        ResetCoroutines();
    }

    public override bool MoveNext()
    {
        var moveBool = Move.MoveNext();
        var sealBool = _seal.MoveNext();

        if (!moveBool)
        {
            MoveTo = Move.MoveTo;
            ConfirmAction();
            return false;
        }

        if (sealBool) return true;

        var currentSealOn = _seal.SelectedElement;
        if (currentSealOn == SealOn)
        {
            SealOn = _seal.SelectedElement;
            MoveTo = Default;
            ConfirmAction();
            return false;
        }

        SealOn = _seal.SelectedElement;
        ResetSeal(Default);

        return true;
    }

    public override void Reset()
    {
        base.Reset();
        ResetCoroutines();
    }

    private void ResetCoroutines()
    {
        SealOn = null;
        ResetSeal(Default);
    }

    private void ResetSeal(Building building)
    {
        _seal = new WaitForSelection<Pathway>(building.GetPathways());
    }
}