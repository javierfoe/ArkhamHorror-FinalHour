using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionCard : MonoBehaviour
{
    private readonly Dictionary<BadAction, string> _badActionTexts = new ();

    [SerializeField] private TMP_Text title, goodAction, badAction;

    public void SetActionCard(ActionDefinition actionDefinition)
    {
        this.title.text = actionDefinition.title;
        this.goodAction.text = actionDefinition.goodAction;
        var badActionString = actionDefinition.badAction;
        var badActionEnum = actionDefinition.badActionEnum;
        if (badActionEnum != BadAction.Special)
        {
            badActionString = "Mueve hasta 1 vez e investiga.\n"+_badActionTexts[badActionEnum];
        }

        this.badAction.text = badActionString;
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
