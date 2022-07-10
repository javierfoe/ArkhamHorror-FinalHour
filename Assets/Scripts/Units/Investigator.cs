using System;
using UnityEngine;

public class Investigator : DwellerGeneric<Location>
{
    private readonly Pool<Action> _actions = new();

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

    [Serializable]
    private class Action
    {
        public int goodActionNumber;
        public bool investigate;
        public BadAction badAction;

        public Action(int number, bool investigate, BadAction badAction)
        {
            goodActionNumber = number;
            this.investigate = investigate;
            this.badAction = badAction;
        }
    }

    private enum BadAction
    {
        MonsterEachPortal,
        TwoMonstersCurrent,
        GreenZone,
        OrangeZone,
        PurpleZone,
        Special
    }
}
