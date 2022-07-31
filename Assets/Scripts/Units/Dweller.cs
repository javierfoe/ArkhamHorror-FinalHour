using UnityEngine;

public abstract class Dweller : MonoBehaviour
{
    [SerializeField] protected Transform hitPoints;
    
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

    protected virtual void Start()
    {
        for (var i = 0; i < hitPoints.childCount - MaxHp; i++)
        {
            hitPoints.GetChild(i).gameObject.SetActive(false);
        }

        for (var i = hitPoints.childCount - MaxHp; i < hitPoints.childCount; i++)
        {
            hitPoints.GetChild(i).gameObject.SetActive(true);
        }
    }
}