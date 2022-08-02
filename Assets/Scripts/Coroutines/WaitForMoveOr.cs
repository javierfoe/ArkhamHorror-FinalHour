public abstract class WaitForMoveOr : WaitFor
{
    protected readonly WaitForMoveUpTo _move;
    protected Building Default => _move.MoveTo;
    public Building MoveTo { get; protected set; }

    protected WaitForMoveOr(Building building, int distance)
    {
        _move = new WaitForMoveUpTo(building, distance);
    }
}
