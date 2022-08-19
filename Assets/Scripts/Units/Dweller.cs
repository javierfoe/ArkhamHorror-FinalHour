using System.Collections;
using UnityEngine;

public abstract class Dweller : MonoBehaviour
{
    protected Transform HitPoints;
    private Location _location;
    public Building Building => Location.Building;
    public int MaxHp { get; protected set; }

    public Location Location => _location;


    public IEnumerator SetLocation(Location location)
    {
        if (_location && _location != location) yield return Empty(_location);
        _location = location;
        if (_location) yield return Fill(_location);
    }


    public virtual IEnumerator Destroy()
    {
#pragma warning disable CS0162
        if (false) yield return null;
#pragma warning restore CS0162
        Destroy(gameObject);
    }

    protected virtual IEnumerator Empty(Location location)
    {
        yield return _location.SetDweller(null);
    }

    protected virtual IEnumerator Fill(Location location)
    {
        yield return _location.SetDweller(this);
    }

    protected virtual void Awake()
    {
        HitPoints = transform.GetChild(1);
    }

    protected virtual void Start()
    {
        for (var i = 0; i < HitPoints.childCount - MaxHp; i++)
        {
            HitPoints.GetChild(i).gameObject.SetActive(false);
        }

        for (var i = HitPoints.childCount - MaxHp; i < HitPoints.childCount; i++)
        {
            HitPoints.GetChild(i).gameObject.SetActive(true);
        }
    }
}