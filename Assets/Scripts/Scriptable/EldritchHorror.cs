using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EldritchHorror", menuName = "ArkhamHorror/EldritchHorror")]
public class EldritchHorror : ScriptableObject
{
    public DifficultySetting easy, normal, hard;
    
    [SerializeField]
    private EldritchMinionDefinition[] monsterDefinitions;

    public EldritchMinionDefinition GetEldritchMinion(EldritchMinion eldritchMinion)
    {
        EldritchMinionDefinition result = null;
        foreach (var monster in monsterDefinitions)
        {
            if (eldritchMinion != monster.eldritchMinion) continue;
            result = monster;
            break;
        }
        return new EldritchMinionDefinition(result);
    }
}

[Serializable]
public class EldritchMinionDefinition
{
    public EldritchMinion eldritchMinion;
    public MonsterDefinition monsterDefinition;

    public EldritchMinionDefinition(EldritchMinionDefinition copy)
    {
        eldritchMinion = copy.eldritchMinion;
        monsterDefinition = new MonsterDefinition(copy.monsterDefinition);
    }
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