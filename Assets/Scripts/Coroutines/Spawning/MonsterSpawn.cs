using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSpawn : IEnumerator
{
    private bool _firstFrame = true;
    private readonly Building _building;
    public Monster Monster { get; private set; }

    public object Current { get; private set; }

    public MonsterSpawn(Monster prefab, MonsterDefinition monsterDefinition, Building building, UnityAction<Monster> onDestroy)
    {
        _building = building;
        Monster = Object.Instantiate(prefab, building.transform).Initialize(monsterDefinition, onDestroy);
    }

    public bool MoveNext()
    {
        if (!_firstFrame) return false;
        Current = _building.IncomingMonster(Monster);
        _firstFrame = false;
        return true;
    }

    public void Reset()
    {
    }
}