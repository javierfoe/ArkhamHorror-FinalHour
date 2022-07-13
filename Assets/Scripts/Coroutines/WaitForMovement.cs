using System.Collections;
using System.Collections.Generic;

public class WaitForMovement : WaitFor
{
    private IEnumerable<Building> _buildings;

    public Building TargetBuilding { get; private set; }

    public WaitForMovement(IEnumerable<Building> buildings)
    {
        _buildings = buildings;
        foreach (var building in _buildings)
        {
            building.OnClick.AddListener(SelectBuilding);
        }
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        foreach (var building in _buildings)
        {
            building.OnClick.RemoveListener(SelectBuilding);
        }
    }

    private void SelectBuilding(Building building)
    {
        TargetBuilding = building;
        ConfirmAction();
    }
}