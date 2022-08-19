using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Investigator : Dweller, IClickable<Investigator>
{
    private readonly Pool<Action> _actions = new();

    [SerializeField] private int card;
    private int _currentHp;

    public UnityEvent<Investigator> OnClick { get; } = new();

    public bool FullHp => _currentHp == MaxHp;
    protected abstract BadAction BadAction { get; }

    public void Initialize(int maxHp)
    {
        MaxHp = maxHp;
    }

    public void Hit(int damage)
    {
        for (var i = 0; i < damage; i++) Hit();
    }

    public ActionCardDefinition DrawCard()
    {
        Action action;
        if (card < 0 || card >= _actions.Count)
        {
            action = _actions[card];
        }
        else
        {
            action = _actions.GetRandom();
        }

        return GetActionCardDefinition(action);
    }

    public void OnMouseDown()
    {
        OnClick.Invoke(this);
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

    protected override void Awake()
    {
        base.Awake();
        
        _actions.Add(new Action(1, BadAction.MonsterEachPortal));
        _actions.Add(new Action(1, BadAction.TwoMonstersCurrent));

        var badActions = new Pool<BadAction>();
        for (var i = 0; i < 2; i++)
        {
            _actions.Add(new Action(0));
            badActions.Add(BadAction.GreenZone);
            badActions.Add(BadAction.OrangeZone);
            badActions.Add(BadAction.PurpleZone);
        }

        float j = 2;
        for (var i = 0; i < 6; i++, j+=0.5f)
        {
            // j is 2,2,3,3,4,4
            _actions.Add(new Action((int)j, badActions.GetRandom()));
        }
    }

    protected override void Start()
    {
        _currentHp = MaxHp;
        base.Start();
    }

    protected abstract GoodAction GetGoodAction(int action);

    private void Hit()
    {
        _currentHp--;
        if (_currentHp == 0) return;
        HitPoints.GetChild(MaxHp - _currentHp - 1).gameObject.SetActive(false);
    }

    private ActionCardDefinition GetActionCardDefinition(Action action)
    {
        var goodAction = GetGoodAction(action.GoodActionNumber);
        var badAction = action.GoodActionNumber != 0 ? action.BadAction : BadAction;

        return new ActionCardDefinition(goodAction, badAction);
    }

    private class Action
    {
        public readonly int GoodActionNumber;
        public readonly BadAction BadAction;

        public Action(int number, BadAction badAction = BadAction.Special)
        {
            GoodActionNumber = number;
            BadAction = badAction;
        }
    }
}