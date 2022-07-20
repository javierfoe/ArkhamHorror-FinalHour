using UnityEngine.Profiling;

public class WaitForMoveAndSeal : WaitFor
{

    private readonly Building _firstBuilding;
    private readonly int _distance;
    private WaitForMoveUpTo _move;
    private WaitForSelection<Pathway> _seal;

    private Building Default => MoveTo ? MoveTo : _firstBuilding;
    public Building MoveTo { get; private set; }
    public Pathway SealOn { get; private set; }
    public bool FirstMove { get; private set; }

    public WaitForMoveAndSeal(Building building, int distance = 1)
    {
        _distance = distance;
        _firstBuilding = building;
        ResetCoroutines();
    }

    public override bool MoveNext()
    {
        if (!base.MoveNext()) return false;
        
        var moveBool = _move.MoveNext();
        var sealBool = _seal.MoveNext();

        if (!moveBool)
        {
            var currentMoveTo = _move.SelectedElement;
            if (MoveTo != _firstBuilding && _firstBuilding == currentMoveTo)
            {
                MoveTo = currentMoveTo;
                Reset();
                return true;
            }

            if (currentMoveTo == MoveTo)
            {
                ConfirmAction();
                return true;
            }
            
            MoveTo = currentMoveTo;
            ResetMove();
        }

        if (!sealBool)
        {
            var currentSealOn = _seal.SelectedElement;
            if (currentSealOn == SealOn)
            {
                SealOn = _seal.SelectedElement;
                MoveTo = Default;
                ConfirmAction();
                return true;
            }
            SealOn = _seal.SelectedElement;
            ResetSeal(Default);
        }
        
        if (!moveBool && sealBool && !FirstMove && MoveTo != _firstBuilding)
        {
            _seal = new WaitForSelection<Pathway>(MoveTo.GetPathways());
            FirstMove = true;
        }

        return moveBool || sealBool;
    }

    public override void Reset()
    {
        base.Reset();
        ResetCoroutines();
    }

    private void ResetCoroutines()
    {
        FirstMove = false;
        SealOn = null;
        ResetMove();
        ResetSeal(_firstBuilding);
    }

    private void ResetSeal(Building building)
    {
        _seal = new WaitForSelection<Pathway>(building.GetPathways());
    }

    private void ResetMove()
    {
        _move = new WaitForMoveUpTo(_firstBuilding, _distance);
    }
}
