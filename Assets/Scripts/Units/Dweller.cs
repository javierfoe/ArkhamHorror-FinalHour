using UnityEngine;

public abstract class Dweller : MonoBehaviour
{
    protected Transform HitPoints;
    private Location _location;
    public Building Building => Location.Building;
    public int MaxHp { get; protected set; }

    public Location Location
    {
        get => _location;
        set
        {
            if (_location && _location != value) Empty(_location);
            _location = value;
            if (_location) Fill(_location);
        }
    }

    protected virtual void Empty(Location location)
    {
        _location.SetDweller(null);
    }

    protected virtual void Fill(Location location)
    {
        _location.SetDweller(this);
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
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