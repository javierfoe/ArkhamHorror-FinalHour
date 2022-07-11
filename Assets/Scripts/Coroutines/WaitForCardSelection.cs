using System.Collections;
using System.Collections.Generic;

public class WaitForCardSelection : WaitFor
{
    public OmenCardDefinition SelectedCard { get; private set; }
    public readonly List<OmenCardDefinition> DiscardedCards;

    public WaitForCardSelection(List<OmenCardDefinition> cards)
    {
        DiscardedCards = cards;
    }

    public void SelectionDone(int index)
    {
        SelectedCard = DiscardedCards[index];
        DiscardedCards.RemoveAt(index);
        ConfirmAction();
    }

    protected override IEnumerator Finished()
    {
        yield return null;
    }
}
