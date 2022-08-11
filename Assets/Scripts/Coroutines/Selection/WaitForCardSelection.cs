using System.Collections.Generic;

public class WaitForCardSelection : WaitForSelection<OmenCard, int, OmenCardDefinition>
{
    public readonly List<OmenCardDefinition> DiscardedCards = new();

    public WaitForCardSelection(IEnumerable<OmenCard> cards) : base(cards)
    {
        foreach (var omenCard in cards)
        {
            DiscardedCards.Add(omenCard.OmenCardDefinition);
        }
    }

    protected override OmenCardDefinition Cast(int element)
    {
        var result = DiscardedCards[element];
        DiscardedCards.RemoveAt(element);
        return result;
    }
}
