using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Monster : DwellerGeneric<Room>
{
    [SerializeField] private UnityEngine.Color blue, red;
    [SerializeField] private Transform skillTokens;

    private MonsterSkill[] _skills;
    private MonsterDefinition _monsterDefinition;
    private UnityAction<Monster> _onDestroy;
    private bool _dead;

    public MonsterDefinition MonsterDefinition
    {
        get => _monsterDefinition;
        private set
        {
            _monsterDefinition = value;
            MaxHp = _monsterDefinition.hp;
            _skills = _monsterDefinition.skills;
        }
    }

    public Color Color => MonsterDefinition.color;
    public MonsterSkill MainMonsterSkill => _skills?.Length > 0 ? _skills[0] : MonsterSkill.None;

    public Monster Initialize(MonsterDefinition monsterDefinition, UnityAction<Monster> onDestroy = null)
    {
        MonsterDefinition = monsterDefinition;
        _onDestroy = onDestroy;
        return this;
    }

    public override void Destroy()
    {
        Location = null;
        _onDestroy?.Invoke(this);
        _dead = true;
        base.Destroy();
    }

    public IEnumerator Activate()
    {
        foreach (var skill in _skills)
        {
            if (_dead) break;
            switch (skill)
            {
                case MonsterSkill.Killer:
                    Kill();
                    break;
                case MonsterSkill.Wrecker:
                    Wreck();
                    break;
                case MonsterSkill.Stalker:
                    Move();
                    break;
            }
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