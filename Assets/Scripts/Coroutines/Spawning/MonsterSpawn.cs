using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MonsterSpawn : IEnumerator
{
    private bool _firstFrame = true;
    public Monster Monster { get; private set; }

    public object Current { get; }

    public MonsterSpawn(Monster prefab, MonsterDefinition monsterDefinition, UnityAction<Monster> onDestroy)
    {
        Monster = Object.Instantiate(prefab).Initialize(monsterDefinition, onDestroy);
    }

    public bool MoveNext()
    {
        if (!_firstFrame) return false;
        _firstFrame = false;
        return true;
    }

    public void Reset()
    {
    }
}