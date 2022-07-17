using System.Collections.Generic;
using UnityEngine;

public class WaitForMovementPathway : WaitFor
{
    public readonly List<Building> Buildings = new();
    public readonly List<Pathway> Pathways = new();
    private readonly Building _firstBuilding;
    private readonly int _maxSteps;
    private bool _firstBuildingClick;
    private int _currentSteps;
    private Building _currentBuilding;

    public WaitForMovementPathway(int steps, Building building)
    {
        _maxSteps = steps;
        _firstBuilding = building;
        _currentBuilding = building;
    }

    public override bool MoveNext()
    {
        if (!base.MoveNext()) return false;
        switch (Current)
        {
            case null:
            {
                var buildings = new List<Building>(Buildings);
                foreach (var building in _currentBuilding.GetAdjacentBuildings(true))
                {
                    if (buildings.Contains(building)) continue;
                    buildings.Add(building);
                }

                if (!buildings.Contains(_firstBuilding))
                {
                    buildings.Add(_firstBuilding);
                }

                Current = new WaitForMovement(buildings);
                break;
            }
            case WaitForMovement movement:
            {
                var building = movement.TargetBuilding;

                if (_firstBuildingClick && building == _currentBuilding && building == _firstBuilding)
                {
                    ConfirmAction();
                    _firstBuildingClick = false;
                    return false;
                }

                _firstBuildingClick = building == _firstBuilding;

                if (building == _currentBuilding && !_firstBuildingClick)
                {
                    ConfirmAction();
                    return false;
                }

                var pathway = _currentBuilding.GetPathwayTo(building);
                if (Pathways.Contains(pathway) || pathway == null || Buildings.Contains(building) && building != _firstBuilding)
                {
                    var nextIndex = Buildings.IndexOf(building) + 1;
                    Buildings.RemoveRange(nextIndex, Buildings.Count - nextIndex);
                    Pathways.RemoveRange(nextIndex, Pathways.Count - nextIndex);
                    SetCurrentBuilding(building);
                    _currentSteps--;
                }
                else if (_currentSteps < _maxSteps && !_firstBuildingClick ||
                         _firstBuildingClick && _currentBuilding != building)
                {
                    Buildings.Add(building);
                    Pathways.Add(_currentBuilding.GetPathwayTo(building));
                    SetCurrentBuilding(building);
                    _currentSteps++;
                }

                if (_firstBuildingClick)
                {
                    SetCurrentBuilding(building);
                }

                Current = null;
                break;
            }
        }

        return true;
    }

    private void SetCurrentBuilding(Building building)
    {
        _currentBuilding = building;
        Current = null;
    }
}