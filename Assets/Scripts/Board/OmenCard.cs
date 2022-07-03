using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OmenCard : MonoBehaviour
{
    [SerializeField] private int number;
    [SerializeField] private GameObject omenSymbol1, omenSymbol2;
    [SerializeField] private SpriteRenderer clueSymbol;
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

        omenSymbol1.SetActive(definition.omens > 0);
        omenSymbol2.SetActive(definition.omens > 1);

        clueSymbol.sprite = OmenSprites.GetSprite(definition.clue);
        number = definition.number;
    }

    private void OnMouseDown()
    {
        if (_onClick != null)
        {
            _onClick();
        }
    }
}

public static class OmenSprites
{
    private static readonly Dictionary<Clue, Sprite> Sprites = new();

    public static Sprite GetSprite(Clue clue)
    {
        return clue == Clue.Key ? null : Sprites[clue];
    }

    public static void Initialize()
    {
        var star = Resources.Load<Sprite>("Icons/star");
        Sprites.Add(Clue.Star, star);
        var diamond = Resources.Load<Sprite>("Icons/diamond");
        Sprites.Add(Clue.Diamond, diamond);
        var clubs = Resources.Load<Sprite>("Icons/clubs");
        Sprites.Add(Clue.Clubs, clubs);
        var hourglass = Resources.Load<Sprite>("Icons/hourglass");
        Sprites.Add(Clue.Hourglass, hourglass);
        var moon = Resources.Load<Sprite>("Icons/moon");
        Sprites.Add(Clue.Moon, moon);
    }
}

public class OmenCardDefinition
{
    public int number, omens;
    public Clue clue;
}