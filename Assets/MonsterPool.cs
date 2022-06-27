using System.Collections.Generic;

public static class MonsterPool
{
    private static readonly List<MonsterDefinition> Pool = new(), Discard = new();
    
    public static void SetStartingPool(IEnumerable<MonsterDefinition> monsterList)
    {
        Pool.AddRange(monsterList);
    }

    public static MonsterDefinition SpawnMonster()
    {
        var count = Pool.Count;
        if (count < 1)
        {
            if (Discard.Count < 1) return null;
            Pool.AddRange(Discard);
            Discard.Clear();
        }

        var random = UnityEngine.Random.Range(0, count - 1);
        var monster = Pool[random];
        Pool.RemoveAt(random);
        return monster;
    }

    public static void MonsterDied(MonsterDefinition monster)
    {
        Discard.Add(monster);
    }
}
