public abstract class WaitForMoveOr : WaitFor
{
    protected readonly WaitForDoubleClickBuilding _move;
    protected Building Default => _move.MoveTo;
    public Building MoveTo { get; protected set; }

    protected WaitForMoveOr(Building building, int distance)
    {
        _move = new WaitForDoubleClickBuilding(building, distance);
    }
}
