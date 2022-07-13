using UnityEngine;
using UnityEngine.Events;

public class OmenCard : MonoBehaviour
{
    public readonly UnityEvent<int> OnClick = new ();
    
    [SerializeField] private int number;
    [SerializeField] private GameObject omenSymbol1, omenSymbol2;
    [SerializeField] private ClueSymbol clueSymbol;

    public OmenCardDefinition OmenCardDefinition { get; private set; }

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
        OnClick.Invoke(transform.GetSiblingIndex());
    }
}

public class OmenCardDefinition
{
    public int Number, Omens;
    public Clue Clue;
}