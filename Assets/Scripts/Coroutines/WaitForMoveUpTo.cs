using UnityEngine.Events;

public class WaitForMoveUpTo : WaitFor
{
    public readonly UnityEvent<Building> OnChangeBuilding = new(), OnRestart = new();
    
    private readonly Building _firstBuilding;
    private readonly int _distance;
    private WaitForSelection<Building> _buildingSelection;
    private Building _selection;

    public Building MoveTo
    {
        get => _selection != null ? _selection : _firstBuilding;
        private set
        {
            _selection = value;
            OnChangeBuilding.Invoke(value);
        }
    }
    
    public WaitForMoveUpTo(Building origin, int distance)
    {
        _distance = distance;
        _firstBuilding = origin;
        ResetMove();
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
                if (MoveTo != _firstBuilding)
                {
                    MoveTo = currentMoveTo;
                    Reset();
                    return true;
                }
            }

            if (currentMoveTo == _selection)
            {
                ConfirmAction();
                return false;
            }
            
            MoveTo = currentMoveTo;
            ResetMove();
        }

        return true;
    }

    private void ResetMove()
    {
        _buildingSelection = new WaitForSelection<Building>(Building.GetDistanceBuildings(_firstBuilding, _distance));
    }
}
