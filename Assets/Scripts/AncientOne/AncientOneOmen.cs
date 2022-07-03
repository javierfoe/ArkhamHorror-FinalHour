using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AncientOneOmen
{
    private readonly DifficultySetting _difficulty;
    protected readonly EldritchHorror EldritchHorror;
    protected readonly ArkhamHorror ArkhamHorror;
    
    private int _omenSymbols;
    protected abstract string EldritchHorrorResource { get; }

    protected AncientOneOmen(Difficulty difficulty, ArkhamHorror arkhamHorror)
    {
        ArkhamHorror = arkhamHorror;
        EldritchHorror = Resources.Load(EldritchHorrorResource) as EldritchHorror;
        _difficulty = difficulty switch
        {
            Difficulty.Normal => EldritchHorror.normal,
            Difficulty.Hard => EldritchHorror.hard,
            _ => EldritchHorror.easy
        };
    }

    public void AddOmenSymbols(int omens)
    {
        _omenSymbols += omens;
    }

    public IEnumerator ActivateOmenSymbols()
    {
        yield return ActivateInterval(_omenSymbols);
        _omenSymbols = 0;
    }

    public IEnumerator SpawnStartingMonsters()
    {
        yield return ArkhamHorror.SpawnMonstersOtherBuildings(_difficulty.monstersOther);
        foreach (var portal in _difficulty.portals)
        {
            var gate = portal.gate;
            yield return ArkhamHorror.SpawnMonstersGate(portal.standardMonsters, gate);
            var eldritchMinions = portal.eldritchMinions;
            var length = eldritchMinions.Length;
            if (length < 1) continue;
            var minionDefinitions = new EldritchMinionDefinition[length];
            for (var i = 0; i < length; i++)
            {
                var eldritchMinion = eldritchMinions[i];
                minionDefinitions[i] = EldritchHorror.GetEldritchMinion(eldritchMinion.eldritchMinion);
                minionDefinitions[i].monsterDefinition.amount = eldritchMinion.amount;
            }

            EldritchMinionsSpawn eldritchMinionsSpawn = ArkhamHorror.SpawnEldritchMinionsGate(minionDefinitions, gate);
            yield return eldritchMinionsSpawn;
            foreach (var monsters in eldritchMinionsSpawn.Monsters)
            {
                AddMonsters(monsters.Key,monsters.Value);
            }
        }
    }

    protected abstract void AddMonsters(EldritchMinion eldritchMinion, List<Monster> monsters);

    protected abstract IEnumerator ActivateInterval(int omenSymbols);
}