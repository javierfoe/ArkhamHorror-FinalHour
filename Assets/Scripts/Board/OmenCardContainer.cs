using System.Collections.Generic;
using UnityEngine;

public class OmenCardContainer : MonoBehaviour
{
    private OmenCard[] _omenCards;
    private List<OmenCardDefinition> _omenCardDefinitions;

    public void SetOmenCards(List<OmenCardDefinition> definitions)
    {
        definitions.Sort((one, two) => one.Number.CompareTo(two.Number));
        _omenCardDefinitions = definitions;
        var length = definitions.Count;
        if (length != _omenCards.Length) return;
        for (var i = 0; i < length; i++)
        {
            var omenCard = _omenCards[i];
            omenCard.SetOmenCard(definitions[i]);
            omenCard.gameObject.SetActive(true);
        }
    }

    public WaitForCardSelection WaitForCardSelection()
    {
        return new WaitForCardSelection(_omenCards);
    }

    private void Awake()
    {
        _omenCards = GetComponentsInChildren<OmenCard>();
        foreach (var omenCard in _omenCards)
        {
            omenCard.gameObject.SetActive(false);
        }
    }
}