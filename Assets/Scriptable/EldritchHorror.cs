using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EldritchHorror", menuName = "ArkhamHorror/EldritchHorror")]
public class EldritchHorror : ScriptableObject
{
    public EldritchMinionDefinition[] monsterDefinitions;
    public DifficultySetting easy, normal, hard;
}


[Serializable]
public class EldritchMinionDefinition : MonsterDefinition
{
    public EldritchMinion eldritchMinion;
}

[Serializable]
public class DifficultySetting
{
    public int monstersOther;
    public StartingMonsters[] portals;
}

[Serializable]
public class EldritchMinionAmount
{
    public EldritchMinion eldritchMinion;
    public int amount;
}

[Serializable]
public class StartingMonsters
{
    public Gate gate;
    public int standardMonsters;
    public EldritchMinionAmount[] eldritchMinions;
}

public enum EldritchMinion
{
    DeepOne,
    StarSpawn,
    GhoulHorde,
    Chthonian
}