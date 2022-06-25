using UnityEngine;

public class Location : MonoBehaviour
{
    private bool _empty = true;
    public Building Building
    {
        get;
        set;
    }
    public virtual bool IsFree => _empty;
    public void SetDweller(Dweller dweller)
    {
        if (!dweller)
        {
            _empty = true;
            return;
        }
        dweller.transform.SetParent(transform);
        dweller.transform.localPosition = Vector3.zero;
        _empty = false;
    }
}
