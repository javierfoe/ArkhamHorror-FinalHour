using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AncientOneOmen
{
    private readonly int[] _variants;
    private readonly DifficultySetting _difficulty;
    private readonly EldritchHorror _eldritchHorror;
    private int _omenSymbols;
    protected abstract string EldritchHorrorResource { get; }

    protected AncientOneOmen(Difficulty difficulty)
    {
        _eldritchHorror = Resources.Load(EldritchHorrorResource) as EldritchHorror;
        _difficulty = difficulty switch
        {
            Difficulty.Normal => _eldritchHorror.normal,
            Difficulty.Hard => _eldritchHorror.hard,
            _ => _eldritchHorror.easy
        };
    }

    public void AddOmenSymbols(int omens)
    {
        _omenSymbols += omens;
    }

    public void ActivateOmenSymbols()
    {
        for (var i = 0; i < _variants.Length; i++)
        {
            if (_variants[i] >= _omenSymbols) continue;
            ActivateInterval(i);
            break;
        }

        _omenSymbols = 0;
    }

    public IEnumerator SpawnStartingMonsters(ArkhamHorror arkhamHorror)
    {
        yield return arkhamHorror.SpawnMonstersOtherBuildings(_difficulty.monstersOther);
        foreach (var portal in _difficulty.portals)
        {
            var gate = portal.gate;
            yield return arkhamHorror.SpawnMonstersGate(portal.standardMonsters, gate);
            var eldritchMinions = portal.eldritchMinions;
            var length = eldritchMinions.Length;
            if (length < 1) continue;
            var minionDefinitions = new EldritchMinionDefinition[length];
            for (var i = 0; i < length; i++)
            {
                var eldritchMinion = eldritchMinions[i];
                minionDefinitions[i] = _eldritchHorror.GetEldritchMinion(eldritchMinion.eldritchMinion);
                minionDefinitions[i].monsterDefinition.amount = eldritchMinion.amount;
            }

            EldritchMinionsSpawn eldritchMinionsSpawn = arkhamHorror.SpawnEldritchMinionsGate(minionDefinitions, gate);
            yield return eldritchMinionsSpawn;
            foreach (var monsters in eldritchMinionsSpawn.Monsters)
            {
                AddMonsters(monsters.Key,monsters.Value);
            }
        }
    }

    protected abstract void AddMonsters(EldritchMinion eldritchMinion, List<Monster> monsters);

    protected abstract void ActivateInterval(int index);
}