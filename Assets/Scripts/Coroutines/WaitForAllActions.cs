using System.Collections.Generic;
using UnityEngine.Events;

public abstract class WaitForAllActions : WaitForMoveOr
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

        Move.OnChangeBuilding.AddListener(UpdateBuilding);
        Move.OnRestart.AddListener(ResetCoroutines);
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

        UpdateSealCoroutine(building);
        UpdateRepairCoroutine(building);
        UpdateMonstersCoroutine(building);
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
        SelectedMonsters.Clear();
        ResetSeal(building);
        ResetRepair(building);
        ResetMonsters(building);
        ResetHeal();
    }
}

public class WaitForMoveSeal : WaitForAllActions
{
    public new Pathway SealOn => base.SealOn;

    public WaitForMoveSeal(Building building, int distance = 1) : base(building, true, false, false, false, distance)
    {
    }
}

public class WaitForMoveSealRepairHeal : WaitForAllActions
{
    public new Pathway SealOn => base.SealOn;
    public new Investigator Heal => base.Heal;
    public new Room RepairOn => base.RepairOn;

    public WaitForMoveSealRepairHeal(Investigator investigator, int distance = 1, int maxActions = 2) : base(investigator, true, true, true,
        false, distance, maxActions)
    {
    }
}

public class WaitForMoveSelectMonsters : WaitForAllActions
{
    private readonly Building _startingBuilding;
    private readonly int _monsterAmount;
    private readonly bool _alwaysStartingBuilding, _adjacent, _monsterSelection;
    public List<Monster> DamagedMonsters => base.SelectedMonsters;

    public WaitForMoveSelectMonsters(Building building, int distance = 1, int monsterAmount = 1) : base(building, false,
        false, false, true, distance)
    {
        _monsterAmount = monsterAmount;
        _monsterSelection = true;
    }

    public WaitForMoveSelectMonsters(Building building, int distance, int damage, bool adjacent = false,
        bool alwaysStartingBuilding = false, bool seal = false) : base(building, distance, damage, seal)
    {
        _adjacent = adjacent;
        if (!alwaysStartingBuilding) return;
        _alwaysStartingBuilding = true;
        _startingBuilding = building;
    }

    protected override void UpdateMonstersCoroutine(Building building)
    {
        if(!_alwaysStartingBuilding) base.UpdateMonstersCoroutine(building);
    }

    protected override WaitForMonsterSelection ResetMonstersCoroutine(Building building)
    {
        if (_monsterSelection) return new WaitForMonsterSelection(new []{building}, 1);
        if (!_adjacent) return base.ResetMonstersCoroutine(building);
        var auxBuilding = _alwaysStartingBuilding ? _startingBuilding : building;
        return new WaitForDamageMonsters(DamageAmount, auxBuilding.GetAdjacentBuildings(), 1);
    }
}

public class WaitForMoveDamageSeal : WaitForMoveSelectMonsters
{
    public new Pathway SealOn => base.SealOn;

    public WaitForMoveDamageSeal(Building building, int distance, int damage, bool adjacent = false) : base(building, distance, damage, adjacent, false, true)
    {
    }
}

public class WaitForMoveDamageSelectAdjacentMonsters : WaitForMoveSelectMonsters
{
    private readonly int _monsterAmount;
    private WaitForMonsterSelection _adjacentMonsters;

    public new List<Monster> SelectedMonsters => _adjacentMonsters.SelectedMonsters;

    public WaitForMoveDamageSelectAdjacentMonsters(Building building, int distance, int damage, int monsterAmount) :
        base(building, distance, damage)
    {
        _monsterAmount = monsterAmount;
    }

    protected override WaitForMonsterSelection ResetMonstersCoroutine(Building building)
    {
        _adjacentMonsters = new WaitForAdjacentMonsterSelection(_monsterAmount, building);
        return base.ResetMonstersCoroutine(building);
    }
}