using System.Collections.Generic;

public class WaitForMovementBuilding : WaitFor
{
    public readonly List<Building> Buildings = new();
    private readonly Building _firstBuilding;
    private readonly int _maxSteps;
    private readonly bool _allSteps;
    protected bool FirstBuildingClick;
    private int _currentSteps;

    protected Building CurrentBuilding { get; private set; }

    public WaitForMovementBuilding(int steps, Building building, bool allSteps = false)
    {
        _allSteps = allSteps;
        _maxSteps = steps;
        _firstBuilding = building;
        CurrentBuilding = building;
    }

    public sealed override bool MoveNext()
    {
        if (!base.MoveNext()) return false;
        switch (Current)
        {
            case null:
            {
                var buildings = new List<Building>(Buildings);
                foreach (var building in CurrentBuilding.GetAdjacentBuildings(true))
                {
                    if (buildings.Contains(building)) continue;
                    buildings.Add(building);
                }

                if (!buildings.Contains(_firstBuilding))
                {
                    buildings.Add(_firstBuilding);
                }

                Current = new WaitForSelection<Building>(buildings);
                break;
            }
            case WaitForSelection<Building> movement:
            {
                var building = movement.SelectedElement;

                if (FirstBuildingClick && building == CurrentBuilding && building == _firstBuilding && !(_allSteps && _currentSteps < _maxSteps))
                {
                    ConfirmAction();
                    FirstBuildingClick = false;
                    return false;
                }

                FirstBuildingClick = building == _firstBuilding;

                if (building == CurrentBuilding && !FirstBuildingClick && !(_allSteps && _currentSteps < _maxSteps))
                {
                    ConfirmAction();
                    return false;
                }

                if (UndoCondition(building))
                {
                    var nextIndex = Buildings.IndexOf(building) + 1;
                    UndoAction(nextIndex);
                    SetCurrentBuilding(building);
                }
                else if (_currentSteps < _maxSteps && !FirstBuildingClick ||
                         FirstBuildingClick && CurrentBuilding != building)
                {
                    NewBuildingAction(building);
                    SetCurrentBuilding(building);
                }

                if (FirstBuildingClick)
                {
                    SetCurrentBuilding(building);
                }

                Current = null;
                break;
            }
        }

        return true;
    }

    protected virtual bool UndoCondition(Building building)
    {
        return Buildings.Contains(building) && building != _firstBuilding;
    }

    protected virtual void UndoAction(int nextIndex)
    {
        Buildings.RemoveRange(nextIndex, Buildings.Count - nextIndex);
        _currentSteps = nextIndex;
    }

    protected virtual void NewBuildingAction(Building building)
    {
        Buildings.Add(building);
        _currentSteps++;
    }

    private void SetCurrentBuilding(Building building)
    {
        CurrentBuilding = building;
        Current = null;
    }
}