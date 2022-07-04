using System;
using UnityEngine;

public class OmenCard : MonoBehaviour
{
    [SerializeField] private int number;
    [SerializeField] private GameObject omenSymbol1, omenSymbol2;
    [SerializeField] private ClueSymbol clueSymbol;
    private Action _onClick;

    public OmenCardDefinition OmenCardDefinition { get; private set; }

    public void SetOnClick(Action action)
    {
        _onClick = action;
    }

    public void ClearOnClick()
    {
        _onClick = null;
    }

    public void SetOmenCard(OmenCardDefinition definition)
    {
        OmenCardDefinition = definition;

        omenSymbol1.SetActive(definition.Omens > 0);
        omenSymbol2.SetActive(definition.Omens > 1);

        clueSymbol.Clue = definition.Clue;
        number = definition.Number;
    }

    private void OnMouseDown()
    {
        if (_onClick != null)
        {
            _onClick();
        }
    }
}

public class OmenCardDefinition
{
    public int Number, Omens;
    public Clue Clue;
}