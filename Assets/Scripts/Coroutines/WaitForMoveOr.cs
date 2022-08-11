public abstract class WaitForMoveOr : WaitFor
{
    protected readonly WaitForDoubleClickBuilding Move;
    protected Building Default => Move.MoveTo;
    public Building MoveTo { get; protected set; }

    protected WaitForMoveOr(Building building, int distance)
    {
        Move = new WaitForDoubleClickBuilding(building, distance);
    }

    protected bool IsOrigin(Building building)
    {
        return Move.IsOrigin(building);
    }
}
