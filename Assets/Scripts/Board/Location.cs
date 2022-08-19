using System.Collections;
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
    public IEnumerator SetDweller(Dweller dweller)
    {
        if (!dweller)
        {
            _empty = true;
        }
        else
        {
            yield return null;
            dweller.transform.SetParent(transform);
            dweller.transform.localPosition = Vector3.back;
            _empty = false;
        }
    }
}
