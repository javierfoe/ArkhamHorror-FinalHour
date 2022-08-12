using System;
using UnityEngine.Events;

public class WaitForDoubleClickBuilding : WaitFor
{
    public readonly UnityEvent<Building> OnChangeBuilding = new(), OnRestart = new();

    protected readonly Building StartingBuilding;
    private readonly int _distance;
    private Func<bool> _condition;
    private WaitForSelection<Building> _buildingSelection;
    private Building _selection;

    public Building SelectedBuilding
    {
        get => _selection != null ? _selection : StartingBuilding;
        protected set
        {
            _selection = value;
            OnChangeBuilding.Invoke(value);
        }
    }
    
    public WaitForDoubleClickBuilding(Building origin, int distance)
    {
        _distance = distance;
        StartingBuilding = origin;
        ResetMove();
    }

    protected void SetCondition(Func<bool> condition)
    {
        _condition = condition;
    }

    public bool IsOrigin(Building building)
    {
        return building == StartingBuilding;
    }

    public override bool MoveNext()
    {
        var moveBool = _buildingSelection.MoveNext();

        if (!moveBool)
        {
            var currentMoveTo = _buildingSelection.SelectedElement;
            if (currentMoveTo == StartingBuilding)
            {
                OnRestart.Invoke(currentMoveTo);
                if (SelectedBuilding != StartingBuilding)
                {
                    SelectedBuilding = currentMoveTo;
                    Reset();
                    return true;
                }
            }

            if (currentMoveTo == _selection && (_condition == null || _condition()))
            {
                ConfirmAction();
                return false;
            }
            
            SelectedBuilding = currentMoveTo;
            ResetMove();
        }

        return true;
    }

    private void ResetMove()
    {
        _buildingSelection = new WaitForSelection<Building>(Building.GetDistanceBuildings(StartingBuilding, _distance));
    }
}
