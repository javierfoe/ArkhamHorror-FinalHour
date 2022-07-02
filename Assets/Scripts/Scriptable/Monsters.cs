using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterList", menuName = "ArkhamHorror/MonsterList")]
public class Monsters : ScriptableObject
{
    public MonsterDefinition[] monsterDefinitions;
}

[Serializable]
public class MonsterDefinition
{
    public int amount, hp;
    public Color color;
    public MonsterSkill[] skills;

    public MonsterDefinition(MonsterDefinition copy)
    {
        amount = copy.amount;
        hp = copy.hp;
        color = copy.color;
        skills = new MonsterSkill[copy.skills.Length];
        Array.Copy(copy.skills, skills, copy.skills.Length);
    }
}