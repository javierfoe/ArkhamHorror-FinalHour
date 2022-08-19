using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Monster : Dweller, IClickable<Monster>
{
    [SerializeField] private UnityEngine.Color blue, red;
    [SerializeField] private Transform skillTokens;

    private MonsterSkill[] _skills;
    private MonsterDefinition _monsterDefinition;
    private UnityAction<Monster> _onDestroy;
    private bool _dead;

    public UnityEvent<Monster> OnClick { get; } = new();

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

    public Room Room => Location as Room;
    public Color Color => MonsterDefinition.color;
    public MonsterSkill MainMonsterSkill => _skills?.Length > 0 ? _skills[0] : MonsterSkill.None;

    public Monster Initialize(MonsterDefinition monsterDefinition, UnityAction<Monster> onDestroy = null)
    {
        MonsterDefinition = monsterDefinition;
        _onDestroy = onDestroy;
        return this;
    }

    public override IEnumerator Destroy()
    {
        if (Building) Building.RemoveMonster(this);

        yield return SetLocation(null);
        _onDestroy?.Invoke(this);
        _dead = true;
        yield return base.Destroy();
    }

    public IEnumerator Activate()
    {
        foreach (var skill in _skills)
        {
            if (_dead) break;
            switch (skill)
            {
                case MonsterSkill.Killer:
                    yield return Kill();
                    break;
                case MonsterSkill.Wrecker:
                    yield return Wreck();
                    break;
                case MonsterSkill.Stalker:
                    yield return Stalk();
                    break;
            }

            yield return null;
        }
    }

    private IEnumerator Wreck()
    {
        if (!Location) yield break;
        yield return Building.Wreck(this);
    }

    private IEnumerator Stalk()
    {
        if (!Location) yield break;
        yield return Building.MoveMonster(this);
    }

    private IEnumerator Kill()
    {
        if (!Location) yield break;
        yield return Building.Kill();
    }

    protected override void Start()
    {
        base.Start();

        if (_skills != null)
        {
            for (var i = 0; i < _skills.Length; i++)
            {
                var skillToken = skillTokens.GetChild(i).GetComponent<Skill>();
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

        sprite.color = Color == Color.Blue ? blue : red;

        gameObject.name = $"{MaxHp} {Color} {MainMonsterSkill}";
    }

    public void OnMouseDown()
    {
        OnClick.Invoke(this);
    }
}