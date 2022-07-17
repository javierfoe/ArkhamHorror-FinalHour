using System.Collections.Generic;

public class WaitForMovementPathway : WaitForMovementBuilding
{
    public readonly List<Pathway> Pathways = new();

    public WaitForMovementPathway(int steps, Building building) : base(steps, building)
    {
    }

    protected override bool UndoCondition(Building building)
    {
        var pathway = CurrentBuilding.GetPathwayTo(building);
        return Pathways.Contains(pathway) || pathway == null && FirstBuildingClick;
    }

    protected override void UndoAction(int nextIndex)
    {
        base.UndoAction(nextIndex);
        Pathways.RemoveRange(nextIndex, Pathways.Count - nextIndex);
    }

    protected override void NewBuildingAction(Building building)
    {
        base.NewBuildingAction(building);
        Pathways.Add(CurrentBuilding.GetPathwayTo(building));
    }
}