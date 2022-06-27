using System;
using System.Collections;
using UnityEngine;

public class Monster : DwellerGeneric<Room>
{
    public enum Skill
    {
        None,
        Killer,
        Wrecker,
        Stalker
    }

    [SerializeField] private UnityEngine.Color blue, red;
    [SerializeField] private Transform skillTokens;

    private Skill[] _skills;
    private MonsterDefinition _monsterDefinition;

    private MonsterDefinition MonsterDefinition
    {
        get => _monsterDefinition;
        set
        {
            _monsterDefinition = value;
            MaxHp = _monsterDefinition.hp;
            _skills = _monsterDefinition.skills;
        }
    }

    public Color Color => MonsterDefinition.color;
    public Skill MainSkill => _skills?.Length > 0 ? _skills[0] : Skill.None;

    public Monster Initialize(MonsterDefinition monsterDefinition)
    {
        MonsterDefinition = monsterDefinition;
        return this;
    }

    public override void Destroy()
    {
        Location = null;
        MonsterPool.MonsterDied(MonsterDefinition);
        ArkhamHorror.AliveMonsters.Remove(this);
        base.Destroy();
    }

    public IEnumerator Activate()
    {
        foreach (var skill in _skills)
        {
            switch (skill)
            {
                case Skill.Killer:
                    Kill();
                    break;
                case Skill.Wrecker:
                    Wreck();
                    break;
                case Skill.Stalker:
                    Move();
                    break;
            }

            Debug.Log($"Skill {skill} activated", gameObject);
            yield return null;
        }
    }

    private void Wreck()
    {
        if (!Location) return;
        Location.Building.Wreck(this);
    }

    private void Move()
    {
        if (!Location) return;
        Location.Building.MoveMonster(this);
    }

    private void Kill()
    {
        if (!Location) return;
        Location.Building.Kill();
    }

    protected override void Start()
    {
        base.Start();
        if (_skills != null)
        {
            for (var i = 0; i < _skills.Length; i++)
            {
                var skillToken = skillTokens.GetChild(i).GetComponent<global::Skill>();
                skillToken.gameObject.SetActive(true);
                skillToken.ChangeSkill(_skills[i]);
            }

            for (var i = _skills.Length; i < skillTokens.childCount; i++)
            {
                skillTokens.GetChild(i).gameObject.SetActive(false);
            }
        }

        var sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite == null) return;

        sprite.color = Color switch
        {
            Color.Blue => blue,
            Color.Red => red,
            _ => throw new ArgumentOutOfRangeException(nameof(Color))
        };
    }
}

public enum Color
{
    Blue,
    Red
}