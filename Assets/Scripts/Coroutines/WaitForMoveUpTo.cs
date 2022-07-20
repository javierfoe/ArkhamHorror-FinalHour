using System.Collections.Generic;
public class WaitForMoveUpTo : WaitForSelection<Building>
{
    public WaitForMoveUpTo(Building origin, int distance) : base(GetDistanceBuildings(origin, distance))
    {
    }
    
    private static IEnumerable<Building> GetDistanceBuildings(Building origin, int distance)
    {
        var result = new List<Building> { origin };
        var auxBuildings = new List<Building>(result);
        for (var i = 0; i < distance; i++)
        {
            var iteration = new List<Building>(auxBuildings);
            auxBuildings.Clear();
            foreach (var auxBuilding in iteration)
            {
                foreach (var adjacent in auxBuilding.GetAdjacentBuildings())
                {
                    if (result.Contains(adjacent)) continue;
                    result.Add(adjacent);
                    auxBuildings.Add(adjacent);
                }
            }
        }
        return result;
    }
}
