using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WaitForMovementPathway : WaitFor
{
    public readonly List<Building> Buildings = new();
    public readonly List<Pathway> Pathways = new();
    private readonly int _maxSteps;
    private int _currentSteps;
    private Building _currentBuilding;

    public WaitForMovementPathway(int steps, Building building)
    {
        _maxSteps = steps;
        _currentBuilding = building;
    }

    public override bool MoveNext()
    {
        if (!base.MoveNext()) return false;
        switch (Current)
        {
            case null:
            {
                Current = new WaitForMovement(_currentBuilding.GetAdjacentBuildings(true));
                break;
            }
            case WaitForMovement movement:
            {
                var building = movement.TargetBuilding;
                if (building == _currentBuilding)
                {
                    ConfirmAction();
                    Debug.Log("Finish");
                    return false;
                }

                var pathway = _currentBuilding.GetPathwayTo(building);
                if (Pathways.Contains(pathway))
                {
                    var nextIndex = Buildings.IndexOf(building) + 1;
                    var pathwayIndex = Pathways.IndexOf(pathway);
                    Buildings.RemoveRange(nextIndex, Buildings.Count - nextIndex);
                    Pathways.RemoveRange(pathwayIndex, Pathways.Count - pathwayIndex);
                    SetCurrentBuilding(building);
                }
                else if (_currentSteps < _maxSteps)
                {
                    Buildings.Add(building);
                    Pathways.Add(_currentBuilding.GetPathwayTo(building));
                    SetCurrentBuilding(building);
                }

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