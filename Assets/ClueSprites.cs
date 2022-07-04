using System.Collections.Generic;
using UnityEngine;

public static class ClueSprites
{
    private static readonly Dictionary<Clue, Sprite> Clues = new();

    public static Sprite GetSprite(Clue clue)
    {
        return Clues[clue];
    }

    public static void Initialize()
    {
        var star = Resources.Load<Sprite>("Icons/star");
        Clues.Add(Clue.Star, star);
        var diamond = Resources.Load<Sprite>("Icons/diamond");
        Clues.Add(Clue.Diamond, diamond);
        var clubs = Resources.Load<Sprite>("Icons/clubs");
        Clues.Add(Clue.Clubs, clubs);
        var hourglass = Resources.Load<Sprite>("Icons/hourglass");
        Clues.Add(Clue.Hourglass, hourglass);
        var moon = Resources.Load<Sprite>("Icons/moon");
        Clues.Add(Clue.Moon, moon);
        var key = Resources.Load<Sprite>("Icons/key");
        Clues.Add(Clue.Key, key);
    }
}