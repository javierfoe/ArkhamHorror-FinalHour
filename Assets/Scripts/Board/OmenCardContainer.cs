using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var waitFor = new WaitForCardSelection(_omenCardDefinitions);
        for (var i = 0; i < _omenCards.Length; i++)
        {
            var omenCard = _omenCards[i];
            var number = i;
            omenCard.SetOnClick(() => SelectedCard(waitFor, number));
        }
        return waitFor;
    }

    private void SelectedCard(WaitForCardSelection waitFor, int selection)
    {
        waitFor.SelectionDone(selection);
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