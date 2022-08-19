using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionCard : MonoBehaviour
{
    private readonly Dictionary<BadAction, string> _badActionTexts = new();
    private readonly Dictionary<GoodAction, Tuple<string, string>> _goodActionTexts = new();

    [SerializeField] private TMP_Text title, goodAction, badAction;

    public void SetActionCard(ActionCardDefinition actionCard)
    {
        title.text = _goodActionTexts[actionCard.GoodAction].Item1;
        goodAction.text = _goodActionTexts[actionCard.GoodAction].Item2;
        var badActionEnum = actionCard.BadAction;
        badAction.text = (badActionEnum != BadAction.Special ? "Mueve hasta 1 vez e investiga.\n" : "") + _badActionTexts[badActionEnum];
    }

    private void Awake()
    {
        _badActionTexts.Add(BadAction.GreenZone, "Activa los monstruos de la zona verde.");
        _badActionTexts.Add(BadAction.OrangeZone, "Activa los monstruos de la zona naranja.");
        _badActionTexts.Add(BadAction.PurpleZone, "Activa los monstruos de la zona morada.");
        _badActionTexts.Add(BadAction.MonsterEachPortal, "Invoca un monstruo en cada portal.");
        _badActionTexts.Add(BadAction.TwoMonstersCurrent, "Invoca dos monstruos en tu ubicación.");
    }
}

public class ActionCardDefinition
{
    public readonly GoodAction GoodAction;
    public readonly BadAction BadAction;        
    
    public bool Investigate => BadAction is BadAction.GreenZone or BadAction.OrangeZone or BadAction.PurpleZone
        or BadAction.MonsterEachPortal or BadAction.TwoMonstersCurrent;

    public ActionCardDefinition(GoodAction goodAction, BadAction badAction)
    {
        GoodAction = goodAction;
        BadAction = badAction;
    }
}