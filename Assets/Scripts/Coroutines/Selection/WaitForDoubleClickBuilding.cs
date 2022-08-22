using System;
using UnityEngine.Events;

public class WaitForDoubleClickBuilding : WaitFor
{
    public readonly UnityEvent<Building> OnChangeBuilding = new(), OnRestart = new();

    protected readonly Building StartingBuilding;
    private readonly int _distance;
    protected Building Selection;
    private Func<bool> _condition;
    private WaitForSelection<Building> _buildingSelection;

    public virtual Building SelectedBuilding
    {
        get => Selection ? Selection : StartingBuilding;
        protected set
        {
            Selection = value;
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
        if (!base.MoveNext()) return false;
        var moveBool = _buildingSelection.MoveNext();

        if (!moveBool)
        {
            var currentMoveTo = _buildingSelection.SelectedElement;
            if (currentMoveTo == Selection && currentMoveTo == StartingBuilding)
            {
                OnRestart.Invoke(currentMoveTo);
                if (SelectedBuilding != StartingBuilding)
                {
                    SelectedBuilding = currentMoveTo;
                    Reset();
                    return true;
                }
            }

            if (currentMoveTo == Selection && (_condition == null || _condition()))
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
