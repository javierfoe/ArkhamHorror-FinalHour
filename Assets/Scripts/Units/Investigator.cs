using System;
using System.Collections.Generic;
using UnityEngine;

public class Investigator : DwellerGeneric<Location>
{
    private readonly Pool<Action> _actions = new();
    private readonly Dictionary<int, ActionText> _actionDefinitions = new();

    private void Awake()
    {
        _actions.Add(new Action(0, false, BadAction.Special));
        _actions.Add(new Action(0, false, BadAction.Special));
        _actions.Add(new Action(1, true, BadAction.MonsterEachPortal));
        _actions.Add(new Action(1, true, BadAction.TwoMonstersCurrent));
        _actions.Add(new Action(2, true, BadAction.GreenZone));
        _actions.Add(new Action(2, true, BadAction.OrangeZone));
        _actions.Add(new Action(3, true, BadAction.OrangeZone));
        _actions.Add(new Action(3, true, BadAction.PurpleZone));
        _actions.Add(new Action(4, true, BadAction.PurpleZone));
        _actions.Add(new Action(4, true, BadAction.GreenZone));
        _actionDefinitions.Add(0, new ActionText("Pues te reviento", "Haz 2 de daño en tu ubicación."));
        _actionDefinitions.Add(1, new ActionText("Por qué tocas?", "Mueve hasta 2 veces."));
        _actionDefinitions.Add(2, new ActionText("APARCAO!", "Repara tu ubicación una vez."));
        _actionDefinitions.Add(3, new ActionText("Capitán salami!", "Sella un camino conectado a tu ubicación."));
        _actionDefinitions.Add(4, new ActionText("Merengue merengue", "Recupera una vida."));
    }

    protected override void Fill(Location location)
    {
        base.Fill(location);
        location.Building.AddInvestigator(this);
    }

    protected override void Empty(Location location)
    {
        base.Empty(location);
        location.Building.RemoveInvestigator(this);
    }

    private class Action
    {
        public readonly int goodActionNumber;
        public readonly bool investigate;
        public readonly BadAction badAction;

        public Action(int number, bool investigate, BadAction badAction)
        {
            goodActionNumber = number;
            this.investigate = investigate;
            this.badAction = badAction;
        }
    }

    private class ActionText
    {
        public readonly string title, goodAction;

        public ActionText(string title, string goodAction)
        {
            this.title = title;
            this.goodAction = goodAction;
        }
    }
}

[Serializable]
public class ActionDefinition
{
    public readonly string title, goodAction, badAction;
    public readonly BadAction badActionEnum;
    
    public ActionDefinition(string title, string goodAction, BadAction badAction, string badActionString = null)
    {
        this.title = title;
        this.goodAction = goodAction;
        this.badAction = badActionString;
        badActionEnum = badAction;
    }
}
