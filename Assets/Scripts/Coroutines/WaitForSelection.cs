using System.Collections.Generic;

public abstract class WaitForSelection<T,U,V> : WaitFor where T : Clickable<U>
{
    private IEnumerable<T> _buildings;

    public V SelectedElement { get; private set; }

    public WaitForSelection(IEnumerable<T> buildings)
    {
        _buildings = buildings;
        foreach (var building in _buildings)
        {
            building.OnClick.AddListener(SelectElement);
        }
    }

    public override void ConfirmAction()
    {
        base.ConfirmAction();
        foreach (var building in _buildings)
        {
            building.OnClick.RemoveListener(SelectElement);
        }
    }

    private void SelectElement(U element)
    {
        SelectedElement = Cast(element);
        ConfirmAction();
    }

    protected abstract V Cast(U element);
}

public class WaitForSelection<T> : WaitForSelection<T, T, T> where T : Clickable<T>
{
    public WaitForSelection(IEnumerable<T> elements): base(elements){}
    protected override T Cast(T element)
    {
        return element;
    }
}