using System.Collections.Generic;
using UnityEngine.Events;

public abstract class WaitForAllActions : WaitForMoveOr
{
    protected readonly bool SealBool, RepairBool, HealBool, DamageBool;
    private readonly int _maxActions, _damageAmount;
    private readonly Investigator _player;
    private int _actions;
    private Room _room;
    private Investigator _investigator;
    private Pathway _pathway;
    private WaitForSelection<Pathway> _seal;
    private WaitForSelection<Room> _repair;
    private WaitForSelection<Investigator> _heal;
    private WaitForDamageMonsters _damage;

    public Pathway SealOn
    {
        get => _pathway;
        private set => _pathway = value;
    }

    public Room RepairOn
    {
        get => _room;
        private set => _room = value;
    }

    public Investigator Heal
    {
        get => _investigator;
        private set => _investigator = value;
    }

    public List<Monster> SelectedMonsters { get; private set; } = new();

    protected WaitForAllActions(Investigator investigator, bool seal, bool repair, bool heal, bool damage,
        int distance, int maxActions = 5) : base(investigator.Building, distance)
    {
        _player = investigator;
        _maxActions = maxActions;
        SealBool = seal;
        RepairBool = repair;
        HealBool = heal;
        DamageBool = damage;

        Move.OnChangeBuilding.AddListener(UpdateBuilding);
        Move.OnRestart.AddListener(ResetCoroutines);
        ResetCoroutines();
    }

    public override bool MoveNext()
    {
        if (!ProcessCoroutine(_repair, ref _room, ResetRepair)) return false;
        if (!ProcessCoroutine(_heal, ref _investigator, ResetHeal)) return false;
        if (!ProcessCoroutine(_seal, ref _pathway, ResetSeal)) return false;

        if (Move.MoveNext()) return true;

        MoveTo = Default;
        ConfirmAction();
        return false;
    }

    public override void Reset()
    {
        base.Reset();
        ResetCoroutines();
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        if (DamageBool)
        {
            SelectedMonsters = _damage.SelectedMonsters;
        }
    }

    protected virtual void ResetRepair(Building building)
    {
        if (!RepairBool) return;
        _repair = new WaitForSelection<Room>(building.GetRepairableRooms());
    }

    protected virtual void ResetSeal(Building building)
    {
        if (!SealBool) return;
        _seal = new WaitForSelection<Pathway>(building.GetPathways());
    }

    protected virtual void ResetDamage(Building building)
    {
        if (!DamageBool) return;
        if (_actions > 0)
        {
            DecreaseActions();
        }

        _damage = new WaitForDamageMonsters(_damageAmount, new[] { building });
        _damage.OnEmptied.AddListener(DecreaseActions);
        _damage.OnNotEmpty.AddListener(IncreaseActions);
    }

    protected virtual void ResetHeal()
    {
        if (!HealBool) return;
        _heal = new WaitForSelection<Investigator>(new[] { _player });
    }

    private bool ProcessCoroutine<T>(WaitForSelection<T> waitFor, ref T element, UnityAction action)
        where T : IClickable<T>
    {
        var waitForBool = waitFor.MoveNext();

        if (waitForBool) return true;

        var selectedElement = waitFor.SelectedElement;
        if (selectedElement.Equals(element))
        {
            element = selectedElement;
            ConfirmAction();
            return false;
        }

        if (_actions >= _maxActions) return true;

        _actions++;
        element = selectedElement;
        action();

        return true;
    }

    private void IncreaseActions()
    {
        _actions++;
    }

    private void DecreaseActions()
    {
        _actions--;
    }

    private void UpdateBuilding(Building building)
    {
        if (_actions >= _maxActions) return;
        if (building != MoveTo && !IsOrigin(building))
        {
            _actions++;
        }

        MoveTo = building;

        _seal = new WaitForSelection<Pathway>(MoveTo.GetPathways());
        _repair = new WaitForSelection<Room>(MoveTo.GetRepairableRooms());
    }

    private void ResetCoroutines()
    {
        ResetCoroutines(Default);
    }

    private void ResetRepair()
    {
        ResetRepair(Default);
    }

    private void ResetSeal()
    {
        ResetSeal(Default);
    }

    private void ResetCoroutines(Building building)
    {
        _actions = 0;
        SealOn = null;
        RepairOn = null;
        Heal = null;
        ResetSeal(building);
        ResetRepair(building);
        ResetDamage(building);
        ResetHeal();
    }
}