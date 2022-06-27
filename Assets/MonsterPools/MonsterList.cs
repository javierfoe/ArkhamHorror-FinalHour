using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterList", menuName = "MonsterList")]
public class MonsterList : ScriptableObject
{
    public MonsterDefinition[] monsterDefinitions;
}

[Serializable]
public class MonsterDefinition
{
    public int amount, hp;
    public Color color;
    public Monster.Skill[] skills;
}