using System;
using UnityEngine.Events;

public class WaitForDoubleClickBuilding : WaitFor
{
    public readonly UnityEvent<Building> OnChangeBuilding = new(), OnRestart = new();

    private readonly Func<bool> _condition;
    private readonly Building _firstBuilding;
    private readonly int _distance;
    private WaitForSelection<Building> _buildingSelection;
    private Building _selection;

    public Building SelectedBuilding
    {
        get => _selection != null ? _selection : _firstBuilding;
        protected set
        {
            _selection = value;
            OnChangeBuilding.Invoke(value);
        }
    }
    
    public WaitForDoubleClickBuilding(Building origin, int distance, Func<bool> condition = null)
    {
        _condition = condition;
        _distance = distance;
        _firstBuilding = origin;
        ResetMove();
    }

    public bool IsOrigin(Building building)
    {
        return building == _firstBuilding;
    }

    public override bool MoveNext()
    {
        var moveBool = _buildingSelection.MoveNext();

        if (!moveBool)
        {
            var currentMoveTo = _buildingSelection.SelectedElement;
            if (currentMoveTo == _firstBuilding)
            {
                OnRestart.Invoke(currentMoveTo);
                if (SelectedBuilding != _firstBuilding)
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
        _buildingSelection = new WaitForSelection<Building>(Building.GetDistanceBuildings(_firstBuilding, _distance));
    }
}
