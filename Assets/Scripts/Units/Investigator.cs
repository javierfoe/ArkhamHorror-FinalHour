using System;
using UnityEngine.Events;

public abstract class Investigator : Dweller, IClickable<Investigator>
{
    private readonly Pool<Action> _actions = new();
    private readonly ActionText[] _actionDefinitions = new ActionText[5];

    private int _currentHp;

    public UnityEvent<Investigator> OnClick { get; } = new();

    public bool FullHp => _currentHp == MaxHp;

    public Investigator Initialize(int maxHp)
    {
        MaxHp = maxHp;
        return this;
    }

    public void Hit(int damage)
    {
        for (var i = 0; i < damage; i++) Hit();
    }

    private void Hit()
    {
        _currentHp--;
        if (_currentHp == 0) return;
        HitPoints.GetChild(MaxHp - _currentHp - 1).gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        
        _actions.Add(new Action(0, false, BadAction.Special));
        _actions.Add(new Action(0, false, BadAction.Special));
        _actions.Add(new Action(1, true, BadAction.MonsterEachPortal));
        _actions.Add(new Action(1, true, BadAction.TwoMonstersCurrent));

        var badActions = new Pool<BadAction>();
        for (var i = 0; i < 2; i++)
        {
            badActions.Add(BadAction.GreenZone);
            badActions.Add(BadAction.OrangeZone);
            badActions.Add(BadAction.PurpleZone);
        }

        float j = 2;
        for (var i = 0; i < 6; i++, j+=0.5f)
        {
            // j is 2,2,3,3,4,4
            _actions.Add(new Action((int)j, true, badActions.GetRandom()));
        }
    }

    protected override void Start()
    {
        _currentHp = MaxHp;
        base.Start();
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

    public void OnMouseDown()
    {
        OnClick.Invoke(this);
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