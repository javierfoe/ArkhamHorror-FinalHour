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
}