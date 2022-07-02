using System.Collections.Generic;

public class Pool<T> : List<T>
{
    private readonly List<T> _discard = new();

    public T GetRandom()
    {
        var count = Count;
        if (count < 1)
        {
            if (_discard.Count < 1) return default;
            AddRange(_discard);
            _discard.Clear();
        }

        var random = UnityEngine.Random.Range(0, count - 1);
        var monster = this[random];
        RemoveAt(random);
        return monster;
    }

    public void Discard(T discard)
    {
        _discard.Add(discard);
    }
}
