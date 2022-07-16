using System.Collections;

public abstract class WaitFor : IEnumerator
{
    private bool confirmed, finishedAssigned;

    public object Current { get; protected set; }

    public virtual void ConfirmAction()
    {
        confirmed = true;
    }

    public virtual bool MoveNext()
    {
        return !confirmed;
    }

    public virtual void Reset()
    {
    }
}