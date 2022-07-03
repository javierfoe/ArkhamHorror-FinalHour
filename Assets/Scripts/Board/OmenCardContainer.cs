using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OmenCardContainer : MonoBehaviour
{
    private OmenCard[] _omenCards;

    public void SetOmenCards(List<OmenCardDefinition> definitions)
    {
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
        var waitFor = new WaitForCardSelection();
        for (var i = 0; i < _omenCards.Length; i++)
        {
            var omenCard = _omenCards[i];
            var number = i;
            omenCard.SetOnClick(() => SelectedCard(waitFor, number));
        }
        return waitFor;
    }

    public void SelectedCard(WaitForCardSelection waitFor, int selection)
    {
        var selected = _omenCards[selection].OmenCardDefinition;
        var rest = _omenCards.Select(omen => omen.OmenCardDefinition).ToList();
        rest.RemoveAt(selection);
        waitFor.SelectionDone(selected, rest);
        foreach (var omenCard in _omenCards)
        {
            omenCard.ClearOnClick();
        }
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