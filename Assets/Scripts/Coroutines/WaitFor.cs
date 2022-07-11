using System.Collections;

public abstract class WaitFor : IEnumerator
{
    private bool confirmed, finishedAssigned;

    public object Current { get; private set; }

    protected WaitFor()
    {
    }

    public void ConfirmAction()
    {
        confirmed = true;
    }

    public bool MoveNext()
    {
        if (finishedAssigned) return false;
        if (!confirmed) return true;
        finishedAssigned = true;
        Current = Finished();
        return true;
    }

    public void Reset()
    {
    }

    protected virtual IEnumerator Finished()
    {
        yield return null;
    }
}