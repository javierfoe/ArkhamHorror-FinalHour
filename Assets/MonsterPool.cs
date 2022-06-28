using System.Collections.Generic;

public class MonsterPool
{
    private readonly List<MonsterDefinition> _pool = new(), _discard = new();
    
    public MonsterPool(IEnumerable<MonsterDefinition> monsterList)
    {
        _pool.AddRange(monsterList);
    }

    public MonsterDefinition SpawnMonster()
    {
        var count = _pool.Count;
        if (count < 1)
        {
            if (_discard.Count < 1) return null;
            _pool.AddRange(_discard);
            _discard.Clear();
        }

        var random = UnityEngine.Random.Range(0, count - 1);
        var monster = _pool[random];
        _pool.RemoveAt(random);
        return monster;
    }

    public void MonsterDied(MonsterDefinition monster)
    {
        _discard.Add(monster);
    }
}
