﻿public class WaitForDamage : WaitForSelectMonsters
{
    private Building _monsterBuilding;

    public override Building SelectedBuilding
    {
        get
        {
            if (!Selection && _monsterBuilding) return _monsterBuilding;
            return base.SelectedBuilding;
        }
        protected set => base.SelectedBuilding = value;
    }

    public WaitForDamage(Building building, int distance, int damage, bool includeSelf = false, bool adjacent = false,
        bool alwaysStartingBuilding = false, bool seal = false) : base(building, distance, damage, includeSelf,
        adjacent, alwaysStartingBuilding, seal)
    {
    }

    public override void Reset()
    {
        base.Reset();
        Monsters.OnEmptied.AddListener(NoMonsters);
        Monsters.OnNotEmpty.AddListener(MonsterSelected);
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        Monsters.ConfirmAction();
    }

    private void NoMonsters()
    {
        _monsterBuilding = null;
    }

    private void MonsterSelected()
    {
        _monsterBuilding = Monsters.Building;
    }
}