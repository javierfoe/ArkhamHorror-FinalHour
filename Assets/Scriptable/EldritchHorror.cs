using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EldritchHorror", menuName = "ArkhamHorror/EldritchHorror")]
public class EldritchHorror : ScriptableObject
{
    public int[] intervalMinimums;
    public DifficultySetting easy, normal, hard;
    
    [SerializeField]
    private EldritchMinionDefinition[] monsterDefinitions;

    public MonsterDefinition GetEldritchMinion(EldritchMinion eldritchMinion)
    {
        MonsterDefinition result = null;
        foreach (var monster in monsterDefinitions)
        {
            if (eldritchMinion != monster.eldritchMinion) continue;
            result = monster.monsterDefinition;
            break;
        }
        return result;
    }
}

[Serializable]
public class Interval
{
    public int min, max;
}

[Serializable]
public class EldritchMinionDefinition
{
    public EldritchMinion eldritchMinion;
    public MonsterDefinition monsterDefinition;
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