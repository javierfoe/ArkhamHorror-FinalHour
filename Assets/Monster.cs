using System;
using System.Collections;
using UnityEngine;

public class Monster : DwellerGeneric<Room>
{
    public enum Skill
    {
        Killer,
        Wrecker,
        Stalker
    }

    [SerializeField] private UnityEngine.Color blue, red;
    [SerializeField] private Color color;
    [SerializeField] private Transform skillTokens;
    [SerializeField] private Skill[] skills;

    public Color Color => color;

    public IEnumerator Activate()
    {
        foreach (var skill in skills)
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

    protected override void Awake()
    {
        base.Awake();
        for (var i = 0; i < skills.Length; i++)
        {
            var skillToken = skillTokens.GetChild(i).GetComponent<global::Skill>();
            skillToken.gameObject.SetActive(true);
            skillToken.ChangeSkill(skills[i]);
        }

        for (var i = skills.Length; i < skillTokens.childCount; i++)
        {
            skillTokens.GetChild(i).gameObject.SetActive(false);
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