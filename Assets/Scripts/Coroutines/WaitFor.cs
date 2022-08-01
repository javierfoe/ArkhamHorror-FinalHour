using System.Collections;

public abstract class WaitFor : IEnumerator
{
    private bool _confirmed;

    public object Current { get; protected set; }

    public virtual void ConfirmAction()
    {
        _confirmed = true;
    }

    public virtual bool MoveNext()
    {
        return !_confirmed;
    }

    public virtual void Reset()
    {
    }
}