using UnityEngine;
using UnityEngine.Events;

public abstract class Clickable<T> : MonoBehaviour
{
    public readonly UnityEvent<T> OnClick = new();

    public void OnMouseDown()
    {
        OnClick.Invoke(InvokeArgument());
    }

    protected abstract T InvokeArgument();
}
