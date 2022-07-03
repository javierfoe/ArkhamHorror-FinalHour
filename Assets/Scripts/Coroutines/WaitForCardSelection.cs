using System.Collections;
using System.Collections.Generic;

public class WaitForCardSelection : IEnumerator
{
    private bool _selectionDone;
    
    public object Current { get; }
    public OmenCardDefinition SelectedCard { get; private set; }
    public readonly List<OmenCardDefinition> DiscardedCards = new();

    public bool MoveNext()
    {
        return !_selectionDone;
    }

    public void SelectionDone(OmenCardDefinition selected, List<OmenCardDefinition> rest)
    {
        SelectedCard = selected;
        DiscardedCards.AddRange(rest);
        _selectionDone = true;
    }

    public void Reset()
    {
    }

}
