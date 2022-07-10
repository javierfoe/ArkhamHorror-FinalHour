using System.Collections.Generic;

public class Ritual
{
    private const int SinglePlayerMinimumClues = 3;
    private readonly List<Clue> _stopRitualClues = new();
    private readonly Clue _first, _second;
    
    public Ritual(Clue one, Clue two)
    {
        _first = one;
        _second = two;
    }

    public void AddClue(Clue stop)
    {
        _stopRitualClues.Add(stop);
    }

    public bool IsRitualStopped()
    {
        return IsRitualStoppedMinimum(SinglePlayerMinimumClues);
    }

    private bool IsRitualStoppedMinimum(int minimumClues)
    {
        var matchingClues = 0;
        foreach (var clue in _stopRitualClues)
        {
            matchingClues += clue == _first || clue == _second ? 1 : 0;
        }

        return matchingClues >= minimumClues;
    }
}
