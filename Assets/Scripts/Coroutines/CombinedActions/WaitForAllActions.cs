using System.Collections.Generic;
using UnityEngine.Events;

public abstract class WaitForAllActions : WaitForDoubleClickBuilding
{
    protected readonly int DamageAmount;
    private readonly bool _sealBool, _repairBool, _healBool, _monstersBool;
    private readonly int _maxActions;
    private readonly Investigator _player;
    private int _actions;
    private Room _room;
    private Investigator _investigator;
    private Pathway _pathway;
    private WaitForSelection<Pathway> _seal;
    private WaitForSelection<Room> _repair;
    private WaitForSelection<Investigator> _heal;
    private WaitForMonsterSelection _monsters;

    protected Pathway SealOn
    {
        get => _pathway;
        private set => _pathway = value;
    }

    protected Room RepairOn
    {
        get => _room;
        private set => _room = value;
    }

    protected Investigator Heal
    {
        get => _investigator;
        private set => _investigator = value;
    }

    protected List<Monster> SelectedMonsters { get; private set; } = new();

    protected WaitForAllActions(Building building, bool seal, bool repair, bool heal, bool monsters,
        int distance, int maxActions = 5, int damage = 0) : base(building, distance)
    {
        DamageAmount = damage;
        _maxActions = maxActions;
        _sealBool = seal;
        _repairBool = repair;
        _healBool = heal;
        _monstersBool = monsters;

        OnChangeBuilding.AddListener(UpdateBuilding);
        OnRestart.AddListener(ResetCoroutines);
        ResetCoroutines();
    }

    protected WaitForAllActions(Investigator investigator, bool seal, bool repair, bool heal, bool monsters,
        int distance, int maxActions = 5, int damage = 0) : this(investigator.Building, seal, repair, heal, monsters,
        distance,
        maxActions, damage)
    {
        _player = investigator;
    }

    protected WaitForAllActions(Building building, int distance, int damage = 0, bool seal = false, int maxActions = 5)
        : this(building,
            seal, false, false, true, distance, maxActions, damage)
    {
    }

    public override bool MoveNext()
    {
        if (!ProcessCoroutine(_repair, ref _room, ResetRepair)) return false;
        if (!ProcessCoroutine(_heal, ref _investigator, ResetHeal)) return false;
        if (!ProcessCoroutine(_seal, ref _pathway, ResetSeal)) return false;

        return base.MoveNext();
    }

    public override void Reset()
    {
        base.Reset();
        ResetCoroutines();
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        if (_monstersBool)
        {
            SelectedMonsters = _monsters.SelectedMonsters;
        }
    }

    protected virtual WaitForSelection<Room> ResetRepairCoroutine(Building building)
    {
        return new WaitForSelection<Room>(building.GetRepairableRooms());
    }

    protected virtual WaitForSelection<Pathway> ResetSealCoroutine(Building building)
    {
        return new WaitForSelection<Pathway>(building.GetPathways());
    }

    protected virtual WaitForMonsterSelection ResetMonstersCoroutine(Building building)
    {
        return new WaitForDamageMonsters(DamageAmount, new[] { building });
    }

    protected virtual WaitForSelection<Investigator> ResetHealCoroutine()
    {
        return new WaitForSelection<Investigator>(new[] { _player });
    }

    protected virtual void UpdateSealCoroutine(Building building)
    {
        _seal = ResetSealCoroutine(building);
    }

    protected virtual void UpdateRepairCoroutine(Building building)
    {
        _repair = ResetRepairCoroutine(building);
    }

    protected virtual void UpdateMonstersCoroutine(Building building)
    {
        _monsters = ResetMonstersCoroutine(building);
    }

    private void ResetRepair(Building building)
    {
        if (!_repairBool) return;
        UpdateRepairCoroutine(building);
    }

    private void ResetSeal(Building building)
    {
        if (!_sealBool) return;
        UpdateSealCoroutine(building);
    }

    private void ResetMonsters(Building building)
    {
        if (!_monstersBool) return;
        if (_actions > 0)
        {
            DecreaseActions();
        }

        UpdateMonstersCoroutine(building);
        _monsters.OnEmptied.AddListener(DecreaseActions);
        _monsters.OnNotEmpty.AddListener(IncreaseActions);
    }

    private void ResetHeal()
    {
        if (!_healBool) return;
        _heal = ResetHealCoroutine();
    }

    private bool ProcessCoroutine<T>(WaitForSelection<T> waitFor, ref T element, UnityAction action)
        where T : IClickable<T>
    {
        if (waitFor == null) return true;
        
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
        if (building != SelectedBuilding && !IsOrigin(building))
        {
            _actions++;
        }

        UpdateSealCoroutine(building);
        UpdateRepairCoroutine(building);
        UpdateMonstersCoroutine(building);
    }

    private void ResetCoroutines()
    {
        ResetCoroutines(SelectedBuilding);
    }

    private void ResetRepair()
    {
        ResetRepair(SelectedBuilding);
    }

    private void ResetSeal()
    {
        ResetSeal(SelectedBuilding);
    }

    private void ResetCoroutines(Building building)
    {
        _actions = 0;
        SealOn = null;
        RepairOn = null;
        Heal = null;
        SelectedMonsters.Clear();
        ResetSeal(building);
        ResetRepair(building);
        ResetMonsters(building);
        ResetHeal();
    }
}