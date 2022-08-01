public class WaitForMoveUpTo : WaitForSelection<Building>
{
    public WaitForMoveUpTo(Building origin, int distance) : base(Building.GetDistanceBuildings(origin, distance))
    {
    }
}
