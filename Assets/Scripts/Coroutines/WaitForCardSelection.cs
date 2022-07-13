using System.Collections;
using System.Collections.Generic;

public class WaitForCardSelection : WaitFor
{
    public OmenCardDefinition SelectedCard { get; private set; }
    public readonly List<OmenCardDefinition> DiscardedCards = new();

    private readonly IEnumerable<OmenCard> _omenCards;

    public WaitForCardSelection(IEnumerable<OmenCard> cards)
    {
        _omenCards = cards;
        foreach (var omenCard in _omenCards)
        {
            omenCard.OnClick.AddListener(SelectCard);
            DiscardedCards.Add(omenCard.OmenCardDefinition);
        }
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        foreach (var omenCard in _omenCards)
        {
            omenCard.OnClick.RemoveListener(SelectCard);
        }
    }

    private void SelectCard(int index)
    {
        SelectedCard = DiscardedCards[index];
        DiscardedCards.RemoveAt(index);
        ConfirmAction();
    }
}
